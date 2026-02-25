using Microsoft.AspNetCore.Mvc;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Controllers;

public sealed class OrderController : Controller
{
    private readonly ISeoMetaService _seo;

    public OrderController(ISeoMetaService seo) => _seo = seo;

    [HttpGet("/order")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/order";
        var meta = await _seo.GetForPageAsync("Order", null, canonical, ct);
        this.ApplyMeta(meta);
        return View();
    }
}

