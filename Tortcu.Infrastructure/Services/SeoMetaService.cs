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
                "Home"     => "Tortcu — Sevgi ilə hazırlanan premium tortlar | Bakı",
                "Products" => "Məhsullar — Tortlar, Keklər, Şirniyyatlar | Tortcu",
                "About"    => "Haqqımızda — Tortcu | Bakıda ev tortu",
                "Gallery"  => "Qalereya — Tortcu | Hazırladığımız tortlar",
                "Contact"  => "Əlaqə — Tortcu | Bakıda tort sifarişi",
                "Order"    => "Online Sifariş — Tortcu | Xüsusi tort sifariş et",
                _          => "Tortcu"
            };
        }

        if (string.IsNullOrWhiteSpace(meta.Description))
        {
            meta.Description = pageType switch
            {
                "Home"     => "Bakıda ev şəraitində hazırlanan premium tortlar, keklər və şirniyyatlar. Hər sifariş fərdi hazırlanır. Online sifariş edin.",
                "Products" => "Tortcu-nun bütün tort və şirniyyat məhsulları. Keklər, ad günü tortları, toy tortları, çikolad tortlar və xüsusi sifarişlər. Bakıda çatdırılma.",
                "About"    => "Tortcu haqqında — Bakıda sevgi ilə hazırlanan premium tortlar. Hər sifariş fərdi hazırlanır, keyfiyyət həmişə ilk yerdədir.",
                "Gallery"  => "Tortcu qaleryası — hazırladığımız tortlar, keklər və şirniyyatların canlı şəkilləri. İlham almaq üçün nəzər salın.",
                "Contact"  => "Tortcu ilə əlaqə — WhatsApp, Instagram və ya email vasitəsilə bizimlə əlaqə saxlayın. Bakıda ən sürətli tort sifarişi.",
                "Order"    => "Online tort sifarişi — Tortcu vasitəsilə özünüzə xüsusi hazırlanmış tort sifariş edin. Bakıda çatdırılma mövcuddur.",
                _          => ""
            };
        }

        if (string.IsNullOrWhiteSpace(meta.Keywords))
        {
            meta.Keywords = pageType switch
            {
                "Home"     => "tort Bakı, premium tort, ev tortu, ad günü tortu, sifarişlə tort, tortcu Bakı, şirniyyat sifariş, toy tortu, doğum günü tortu",
                "Products" => "tort məhsulları, şirniyyat, Bakıda tort, sifarişlə tort, keks, çikolad tortu, ad günü tortu, toy tortu, makaron",
                "About"    => "Tortcu haqqında, Bakı tortçu, ev tortu, premium şirniyyat, tortcu baku, peşəkar tortçu",
                "Gallery"  => "tort şəkilləri, Bakı tortu, tortcu qalereya, şirniyyat şəkilləri, tort dizayn Bakı",
                "Contact"  => "tortcu əlaqə, Bakı tort sifariş, WhatsApp tort sifariş, tortcu baku əlaqə, tort çatdırılma Bakı",
                "Order"    => "online tort sifariş, tort sifarişi Bakı, fərdi tort, xüsusi tort sifariş, ad günü tortu sifariş",
                _          => ""
            };
        }

        meta.OgTitle ??= meta.Title;
        meta.OgDescription ??= meta.Description;

        return meta;
    }
}

