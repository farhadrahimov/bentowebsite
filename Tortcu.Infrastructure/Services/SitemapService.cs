using System.Globalization;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;

namespace Tortcu.Infrastructure.Services;

public sealed class SitemapService : ISitemapService
{
    private readonly AppDbContext _db;

    public SitemapService(AppDbContext db) => _db = db;

    public async Task<string> GenerateXmlAsync(string baseUrl, CancellationToken ct)
    {
        baseUrl = baseUrl.TrimEnd('/');

        var urls = new List<(string loc, DateTime? lastmodUtc, string changefreq, decimal priority)>
        {
            ($"{baseUrl}/", null, "weekly", 1.0m),
            ($"{baseUrl}/products", null, "weekly", 0.9m),
            ($"{baseUrl}/about", null, "monthly", 0.7m),
            ($"{baseUrl}/gallery", null, "monthly", 0.6m),
            ($"{baseUrl}/contact", null, "monthly", 0.6m),
            ($"{baseUrl}/order", null, "monthly", 0.4m),
        };

        var categories = await _db.Categories
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new { x.Slug })
            .ToListAsync(ct);

        foreach (var c in categories)
        {
            urls.Add(($"{baseUrl}/products/{c.Slug}", null, "weekly", 0.8m));
        }

        var products = await _db.Products
            .AsNoTracking()
            .Where(x => x.IsActive)
            .Select(x => new { x.Slug, x.CreatedAtUtc })
            .ToListAsync(ct);

        foreach (var p in products)
        {
            urls.Add(($"{baseUrl}/products/{p.Slug}", p.CreatedAtUtc, "weekly", 0.7m));
        }

        var sb = new StringBuilder();
        sb.AppendLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
        sb.AppendLine(@"<urlset xmlns=""http://www.sitemaps.org/schemas/sitemap/0.9"">");

        foreach (var u in urls.DistinctBy(x => x.loc))
        {
            sb.AppendLine("  <url>");
            sb.AppendLine($"    <loc>{EscapeXml(u.loc)}</loc>");
            if (u.lastmodUtc is not null)
            {
                sb.AppendLine($"    <lastmod>{u.lastmodUtc.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)}</lastmod>");
            }
            sb.AppendLine($"    <changefreq>{u.changefreq}</changefreq>");
            sb.AppendLine($"    <priority>{u.priority.ToString(CultureInfo.InvariantCulture)}</priority>");
            sb.AppendLine("  </url>");
        }

        sb.AppendLine("</urlset>");
        return sb.ToString();
    }

    private static string EscapeXml(string s)
        => s
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&apos;");
}

