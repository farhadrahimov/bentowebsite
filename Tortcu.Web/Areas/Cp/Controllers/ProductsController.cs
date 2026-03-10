using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Areas.Cp.Controllers;

[Area("Cp")]
[Route("cp/products")]
[Authorize(Roles = "Admin")]
public sealed class ProductsController : Controller
{
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

    private readonly AppDbContext _db;
    private readonly ISlugService _slug;
    private readonly IWebHostEnvironment _env;

    public ProductsController(AppDbContext db, ISlugService slug, IWebHostEnvironment env)
    {
        _db = db;
        _slug = slug;
        _env = env;
    }

    private async Task<string?> SaveProductImageAsync(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0 || file.Length > MaxFileSizeBytes) return null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext)) return null;

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "products");
        Directory.CreateDirectory(uploadDir);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(uploadDir, fileName);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return $"/uploads/products/{fileName}";
    }

    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] int? categoryId, CancellationToken ct)
    {
        var query = _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.DisplayOrder))
            .AsQueryable();
        if (categoryId.HasValue && categoryId.Value > 0)
            query = query.Where(p => p.CategoryId == categoryId.Value);
        var list = await query.OrderByDescending(p => p.CreatedAtUtc).ToListAsync(ct);
        var categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync(ct);
        ViewData["Categories"] = categories;
        ViewData["SelectedCategoryId"] = categoryId;
        return View(list);
    }

    [HttpGet("create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync(ct);
        if (categories.Count == 0)
        {
            TempData["Error"] = "Əvvəlcə ən azı bir kateqoriya yaradın.";
            return RedirectToAction(nameof(Index));
        }
        ViewData["Categories"] = categories;
        return View();
    }

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Create(
        [FromForm] int categoryId,
        [FromForm] string name,
        [FromForm] string? slug,
        [FromForm] string? description,
        [FromForm] decimal price,
        [FromForm] bool isActive,
        [FromForm] bool isPopular,
        [FromForm] string? imageUrl,
        [FromForm] IFormFile? imageFile,
        [FromForm] bool showInGallery,
        CancellationToken ct)
    {
        var categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync(ct);
        ViewData["Categories"] = categories;

        name = (name ?? "").Trim();
        if (string.IsNullOrEmpty(name))
        {
            ViewData["Error"] = "Məhsul adı boş ola bilməz.";
            return View();
        }

        var cat = await _db.Categories.FindAsync(new object[] { categoryId }, ct);
        if (cat == null)
        {
            ViewData["Error"] = "Seçilmiş kateqoriya tapılmadı.";
            return View();
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? _slug.Slugify(name) : _slug.Slugify(slug);
        if (string.IsNullOrEmpty(finalSlug)) finalSlug = "mehsul";

        if (await _db.Products.AnyAsync(p => p.Slug == finalSlug, ct))
        {
            var baseSlug = finalSlug;
            var n = 1;
            while (await _db.Products.AnyAsync(p => p.Slug == finalSlug, ct))
                finalSlug = $"{baseSlug}-{++n}";
        }

        var product = new Tortcu.Domain.Product
        {
            CategoryId = categoryId,
            Name = name,
            Slug = finalSlug,
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim(),
            Price = price < 0 ? 0 : price,
            IsActive = isActive,
            IsPopular = isPopular
        };
        _db.Products.Add(product);
        await _db.SaveChangesAsync(ct);

        var imgUrl = (string?)null;
        if (imageFile is { Length: > 0 })
            imgUrl = await SaveProductImageAsync(imageFile, ct);
        if (string.IsNullOrEmpty(imgUrl))
            imgUrl = (imageUrl ?? "").Trim();
        if (!string.IsNullOrEmpty(imgUrl))
        {
            _db.ProductImages.Add(new Tortcu.Domain.ProductImage
            {
                ProductId = product.Id,
                ImageUrl = imgUrl.Length > 600 ? imgUrl[..600] : imgUrl,
                IsPrimary = true,
                DisplayOrder = 0,
                ShowInGallery = showInGallery
            });
            await _db.SaveChangesAsync(ct);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var p = await _db.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p == null) return NotFound();
        var categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync(ct);
        ViewData["Categories"] = categories;
        return View(p);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Edit(
        int id,
        [FromForm] int categoryId,
        [FromForm] string name,
        [FromForm] string? slug,
        [FromForm] string? description,
        [FromForm] decimal price,
        [FromForm] bool isActive,
        [FromForm] bool isPopular,
        [FromForm] string? imageUrl,
        [FromForm] IFormFile? imageFile,
        [FromForm] bool showInGallery,
        CancellationToken ct)
    {
        var p = await _db.Products.Include(x => x.Images).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (p == null) return NotFound();

        var categories = await _db.Categories.Where(c => c.IsActive).OrderBy(c => c.DisplayOrder).ThenBy(c => c.Name).ToListAsync(ct);
        ViewData["Categories"] = categories;

        name = (name ?? "").Trim();
        if (string.IsNullOrEmpty(name))
        {
            ViewData["Error"] = "Məhsul adı boş ola bilməz.";
            return View(p);
        }

        var cat = await _db.Categories.FindAsync(new object[] { categoryId }, ct);
        if (cat == null)
        {
            ViewData["Error"] = "Seçilmiş kateqoriya tapılmadı.";
            return View(p);
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? _slug.Slugify(name) : _slug.Slugify(slug);
        if (string.IsNullOrEmpty(finalSlug)) finalSlug = "mehsul";

        if (await _db.Products.AnyAsync(x => x.Slug == finalSlug && x.Id != id, ct))
        {
            var baseSlug = finalSlug;
            var n = 1;
            while (await _db.Products.AnyAsync(x => x.Slug == finalSlug && x.Id != id, ct))
                finalSlug = $"{baseSlug}-{++n}";
        }

        p.CategoryId = categoryId;
        p.Name = name;
        p.Slug = finalSlug;
        p.Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        p.Price = price < 0 ? 0 : price;
        p.IsActive = isActive;
        p.IsPopular = isPopular;

        var imgUrl = (string?)null;
        if (imageFile is { Length: > 0 })
            imgUrl = await SaveProductImageAsync(imageFile, ct);
        if (string.IsNullOrEmpty(imgUrl))
            imgUrl = (imageUrl ?? "").Trim();

        var primaryImg = p.Images.FirstOrDefault(i => i.IsPrimary);
        if (!string.IsNullOrEmpty(imgUrl))
        {
            var url = imgUrl.Length > 600 ? imgUrl[..600] : imgUrl;
            if (primaryImg != null)
            {
                primaryImg.ImageUrl = url;
                primaryImg.ShowInGallery = showInGallery;
            }
            else
            {
                _db.ProductImages.Add(new Tortcu.Domain.ProductImage
                {
                    ProductId = p.Id,
                    ImageUrl = url,
                    IsPrimary = true,
                    DisplayOrder = 0,
                    ShowInGallery = showInGallery
                });
            }
        }
        else if (primaryImg != null)
        {
            _db.ProductImages.Remove(primaryImg);
        }

        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var p = await _db.Products.FindAsync(new object[] { id }, ct);
        if (p == null) return NotFound();
        _db.Products.Remove(p);
        await _db.SaveChangesAsync(ct);
        TempData["Success"] = "Məhsul silindi.";
        return RedirectToAction(nameof(Index));
    }
}
