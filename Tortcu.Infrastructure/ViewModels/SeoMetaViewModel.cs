namespace Tortcu.Infrastructure.ViewModels;

public sealed class SeoMetaViewModel
{
    public string Title { get; set; } = "";
    public string? Description { get; set; }
    public string? Keywords { get; set; }

    public string? OgTitle { get; set; }
    public string? OgDescription { get; set; }
    public string? OgImageUrl { get; set; }
    public string? CanonicalUrl { get; set; }
}

