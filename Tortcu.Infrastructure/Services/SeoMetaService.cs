using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;
using Tortcu.Infrastructure.ViewModels;

namespace Tortcu.Infrastructure.Services;

public sealed class SeoMetaService : ISeoMetaService
{
    private readonly AppDbContext _db;

    public SeoMetaService(AppDbContext db) => _db = db;

    public async Task<SeoMetaViewModel> GetForPageAsync(string pageType, int? entityId, string? canonicalUrl, CancellationToken ct)
    {
        var meta = await _db.SeoMetas
            .AsNoTracking()
            .Where(x => x.PageType == pageType && x.EntityId == entityId)
            .Select(x => new SeoMetaViewModel
            {
                Title = x.MetaTitle ?? "",
                Description = x.MetaDescription,
                Keywords = x.MetaKeywords,
                OgTitle = x.OgTitle,
                OgDescription = x.OgDescription,
                OgImageUrl = x.OgImageUrl
            })
            .FirstOrDefaultAsync(ct);

        meta ??= new SeoMetaViewModel();
        meta.CanonicalUrl = canonicalUrl;

        if (string.IsNullOrWhiteSpace(meta.Title))
        {
            meta.Title = pageType switch
            {
                "Home" => "Tortcu — Sevgi ilə hazırlanan premium tortlar",
                "Products" => "Məhsullar — Tortcu",
                "About" => "Haqqımızda — Tortcu",
                "Gallery" => "Qalereya — Tortcu",
                "Contact" => "Əlaqə — Tortcu",
                "Order" => "Online Sifariş — Tortcu",
                _ => "Tortcu"
            };
        }

        meta.OgTitle ??= meta.Title;
        meta.OgDescription ??= meta.Description;

        return meta;
    }
}

