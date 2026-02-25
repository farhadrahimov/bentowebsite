using Microsoft.AspNetCore.Mvc;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Controllers;

public sealed class SitemapController : Controller
{
    private readonly ISitemapService _sitemap;

    public SitemapController(ISitemapService sitemap) => _sitemap = sitemap;

    [HttpGet("/sitemap.xml")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var xml = await _sitemap.GenerateXmlAsync(baseUrl, ct);
        return Content(xml, "application/xml");
    }
}

