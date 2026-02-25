using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Controllers;

public sealed class AboutController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISeoMetaService _seo;

    public AboutController(AppDbContext db, ISeoMetaService seo)
    {
        _db = db;
        _seo = seo;
    }

    [HttpGet("/about")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/about";
        var meta = await _seo.GetForPageAsync("About", null, canonical, ct);
        this.ApplyMeta(meta);

        var content = await _db.AboutContents
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Select(x => new { x.Content, x.MainImageUrl })
            .FirstOrDefaultAsync(ct);

        ViewData["AboutText"] = content?.Content
            ?? "Tortcu — sevgi ilə hazırlanan premium tortlar. Kontent sonradan əlavə ediləcək.";
        ViewData["AboutMainImageUrl"] = content?.MainImageUrl;

        return View();
    }
}

