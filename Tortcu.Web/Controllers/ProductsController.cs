using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;
using Tortcu.Web.ViewModels;

namespace Tortcu.Web.Controllers;

public sealed class ProductsController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISeoMetaService _seo;

    public ProductsController(AppDbContext db, ISeoMetaService seo)
    {
        _db = db;
        _seo = seo;
    }

    [HttpGet("/products")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/products";
        var meta = await _seo.GetForPageAsync("Products", null, canonical, ct);
        this.ApplyMeta(meta);

        var categories = await GetActiveCategoriesAsync(ct);
        var products = await GetProductsAsync(categoryId: null, ct);

        return View(new ProductsIndexViewModel
        {
            Categories = categories,
            Products = products
        });
    }

    // PRD tələb edir:
    // - /products/{category-slug}
    // - /products/{product-slug}
    // Route ambiguity-ni DB lookup ilə həll edirik: əvvəl category, sonra product.
    [HttpGet("/products/{slug}")]
    public async Task<IActionResult> BySlug(string slug, CancellationToken ct)
    {
        slug = (slug ?? "").Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(slug)) return RedirectToAction(nameof(Index));

        var category = await _db.Categories.AsNoTracking()
            .Where(x => x.IsActive && x.Slug == slug)
            .Select(x => new { x.Id, x.Name, x.Slug })
            .FirstOrDefaultAsync(ct);

        if (category is not null)
        {
            var canonical = $"{Request.Scheme}://{Request.Host}/products/{category.Slug}";
            var meta = await _seo.GetForPageAsync("Category", category.Id, canonical, ct);
            this.ApplyMeta(meta);

            var categories = await GetActiveCategoriesAsync(ct);
            var products = await GetProductsAsync(category.Id, ct);

            return View("Index", new ProductsIndexViewModel
            {
                CategoryName = category.Name,
                CategorySlug = category.Slug,
                Categories = categories,
                Products = products
            });
        }

        var product = await _db.Products
            .AsNoTracking()
            .Where(x => x.IsActive && x.Slug == slug)
            .Select(x => new ProductDetailViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Slug = x.Slug,
                Description = x.Description,
                Price = x.Price,
                CategoryName = _db.Categories.Where(c => c.Id == x.CategoryId).Select(c => c.Name).FirstOrDefault(),
                Images = x.Images
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenBy(i => i.DisplayOrder)
                    .Select(i => new ProductImageViewModel { Url = i.ImageUrl, IsPrimary = i.IsPrimary })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (product is null) return NotFound();

        var canonicalProduct = $"{Request.Scheme}://{Request.Host}/products/{product.Slug}";
        var productMeta = await _seo.GetForPageAsync("Product", product.Id, canonicalProduct, ct);
        if (string.IsNullOrWhiteSpace(productMeta.Description) && !string.IsNullOrWhiteSpace(product.Description))
        {
            productMeta.Description = product.Description.Length <= 180 ? product.Description : product.Description[..180];
        }
        productMeta.Title = $"{product.Name} — tort | Tortcu";
        this.ApplyMeta(productMeta);

        return View("Detail", product);
    }

    private Task<List<CategoryCardViewModel>> GetActiveCategoriesAsync(CancellationToken ct)
        => _db.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryCardViewModel { Name = x.Name, Slug = x.Slug })
            .ToListAsync(ct);

    private Task<List<ProductCardViewModel>> GetProductsAsync(int? categoryId, CancellationToken ct)
    {
        var q = _db.Products.AsNoTracking().Where(x => x.IsActive);
        if (categoryId is not null) q = q.Where(x => x.CategoryId == categoryId);

        return q
            .OrderByDescending(x => x.IsPopular)
            .ThenByDescending(x => x.CreatedAtUtc)
            .Select(x => new ProductCardViewModel
            {
                Name = x.Name,
                Slug = x.Slug,
                Price = x.Price,
                ThumbnailUrl = x.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault(),
                CategoryName = _db.Categories.Where(c => c.Id == x.CategoryId).Select(c => c.Name).FirstOrDefault()
            })
            .ToListAsync(ct);
    }
}

