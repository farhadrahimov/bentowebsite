using Microsoft.AspNetCore.Mvc;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Controllers;

public sealed class GalleryController : Controller
{
    private readonly ISeoMetaService _seo;

    public GalleryController(ISeoMetaService seo) => _seo = seo;

    [HttpGet("/gallery")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/gallery";
        var meta = await _seo.GetForPageAsync("Gallery", null, canonical, ct);
        this.ApplyMeta(meta);

        // Placeholder list (real images later)
        var images = Enumerable.Range(1, 18).Select(i => $"/images/gallery/placeholder-{i:00}.webp").ToList();
        return View(images);
    }
}

