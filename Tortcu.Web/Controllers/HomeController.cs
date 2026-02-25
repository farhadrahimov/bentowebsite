using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.Services;
using Tortcu.Web.ViewModels;

namespace Tortcu.Web.Controllers;

public sealed class HomeController : Controller
{
    private readonly AppDbContext _db;
    private readonly ISeoMetaService _seo;

    public HomeController(AppDbContext db, ISeoMetaService seo)
    {
        _db = db;
        _seo = seo;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var canonical = $"{Request.Scheme}://{Request.Host}/";
        var meta = await _seo.GetForPageAsync("Home", null, canonical, ct);
        this.ApplyMeta(meta);

        var campaign = await _db.Campaigns
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderByDescending(x => x.StartDateUtc ?? DateTime.MinValue)
            .Select(x => new CampaignViewModel
            {
                Title = x.Title,
                SubTitle = x.SubTitle,
                ImageUrl = x.ImageUrl
            })
            .FirstOrDefaultAsync(ct);

        var categories = await _db.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new CategoryCardViewModel
            {
                Name = x.Name,
                Slug = x.Slug
            })
            .Take(8)
            .ToListAsync(ct);

        var popular = await _db.Products
            .AsNoTracking()
            .Where(x => x.IsActive && x.IsPopular)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ProductCardViewModel
            {
                Name = x.Name,
                Slug = x.Slug,
                Price = x.Price,
                ThumbnailUrl = x.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault(),
                CategoryName = _db.Categories.Where(c => c.Id == x.CategoryId).Select(c => c.Name).FirstOrDefault()
            })
            .Take(8)
            .ToListAsync(ct);

        var vm = new HomeViewModel
        {
            Campaign = campaign,
            Categories = categories,
            PopularProducts = popular
        };

        return View(vm);
    }

    [HttpGet("/error")]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() => View();
}

