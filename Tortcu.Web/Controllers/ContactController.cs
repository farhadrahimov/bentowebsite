using Microsoft.AspNetCore.Mvc;
using Tortcu.Infrastructure.Services;
using Tortcu.Web.ViewModels;

namespace Tortcu.Web.Controllers;

public sealed class ContactController : Controller
{
    private readonly ISeoMetaService _seo;

    public ContactController(ISeoMetaService seo) => _seo = seo;

    [HttpGet("/contact")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/contact";
        var meta = await _seo.GetForPageAsync("Contact", null, canonical, ct);
        this.ApplyMeta(meta);

        var vm = new ContactViewModel();

        return View(vm);
    }
}

