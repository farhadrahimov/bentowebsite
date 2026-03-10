using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Controllers;

public sealed class GalleryController : Controller
{
    private readonly ISeoMetaService _seo;
    private readonly AppDbContext _db;

    public GalleryController(ISeoMetaService seo, AppDbContext db)
    {
        _seo = seo;
        _db = db;
    }

    [HttpGet("/gallery")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/gallery";
        var meta = await _seo.GetForPageAsync("Gallery", null, canonical, ct);
        this.ApplyMeta(meta);

        var images = await _db.ProductImages
            .AsNoTracking()
            .Where(x => x.ShowInGallery)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Id)
            .Select(x => x.ImageUrl)
            .ToListAsync(ct);

        return View(images);
    }
}

