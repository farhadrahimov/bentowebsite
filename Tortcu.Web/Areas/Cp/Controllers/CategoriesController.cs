using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;
using Tortcu.Web.Areas.Cp.Models;

namespace Tortcu.Web.Areas.Cp.Controllers;

[Area("Cp")]
[Route("cp/categories")]
[Authorize(Roles = "Admin")]
public sealed class CategoriesController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISlugService _slug;

    public CategoriesController(AppDbContext db, ISlugService slug)
    {
        _db = db;
        _slug = slug;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var categories = await _db.Categories
            .Include(c => c.Products)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Name)
            .ToListAsync(ct);
        var list = categories.Select(c => new CategoryListModel(c.Id, c.Name, c.Slug, c.IsActive, c.DisplayOrder, c.Products.Count)).ToList();
        return View(list);
    }

    [HttpGet("create")]
    public IActionResult Create() => View();

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [FromForm] string name,
        [FromForm] string? slug,
        [FromForm] bool isActive,
        [FromForm] int displayOrder,
        CancellationToken ct)
    {
        name = (name ?? "").Trim();
        if (string.IsNullOrEmpty(name))
        {
            ViewData["Error"] = "Kateqoriya adı boş ola bilməz.";
            return View();
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? _slug.Slugify(name) : _slug.Slugify(slug);
        if (string.IsNullOrEmpty(finalSlug))
            finalSlug = "kateqoriya";

        if (await _db.Categories.AnyAsync(c => c.Slug == finalSlug, ct))
        {
            var baseSlug = finalSlug;
            var n = 1;
            while (await _db.Categories.AnyAsync(c => c.Slug == finalSlug, ct))
                finalSlug = $"{baseSlug}-{++n}";
        }

        _db.Categories.Add(new Tortcu.Domain.Category
        {
            Name = name,
            Slug = finalSlug,
            IsActive = isActive,
            DisplayOrder = displayOrder
        });
        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var c = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [FromForm] string name,
        [FromForm] string? slug,
        [FromForm] bool isActive,
        [FromForm] int displayOrder,
        CancellationToken ct)
    {
        var c = await _db.Categories.FindAsync(new object[] { id }, ct);
        if (c == null) return NotFound();

        name = (name ?? "").Trim();
        if (string.IsNullOrEmpty(name))
        {
            ViewData["Error"] = "Kateqoriya adı boş ola bilməz.";
            return View(c);
        }

        var finalSlug = string.IsNullOrWhiteSpace(slug) ? _slug.Slugify(name) : _slug.Slugify(slug);
        if (string.IsNullOrEmpty(finalSlug)) finalSlug = "kateqoriya";

        if (await _db.Categories.AnyAsync(x => x.Slug == finalSlug && x.Id != id, ct))
        {
            var baseSlug = finalSlug;
            var n = 1;
            while (await _db.Categories.AnyAsync(x => x.Slug == finalSlug && x.Id != id, ct))
                finalSlug = $"{baseSlug}-{++n}";
        }

        c.Name = name;
        c.Slug = finalSlug;
        c.IsActive = isActive;
        c.DisplayOrder = displayOrder;
        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var c = await _db.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (c == null) return NotFound();
        if (c.Products.Count > 0)
        {
            TempData["Error"] = $"Kateqoriyada {c.Products.Count} məhsul var. Əvvəlcə məhsulları silin və ya başqa kateqoriyaya köçürün.";
            return RedirectToAction(nameof(Index));
        }
        _db.Categories.Remove(c);
        await _db.SaveChangesAsync(ct);
        TempData["Success"] = "Kateqoriya silindi.";
        return RedirectToAction(nameof(Index));
    }
}
