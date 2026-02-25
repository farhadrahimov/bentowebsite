namespace Tortcu.Domain;

public sealed class SeoMeta
{
    public int Id { get; set; }

    public string PageType { get; set; } = "";
    public int? EntityId { get; set; }

    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
}

