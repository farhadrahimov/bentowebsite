namespace Tortcu.Web.ViewModels;

public sealed class HomeViewModel
{
    public CampaignViewModel? Campaign { get; set; }
    public List<ProductCardViewModel> PopularProducts { get; set; } = new();
    public List<CategoryCardViewModel> Categories { get; set; } = new();
}

public sealed class CampaignViewModel
{
    public string Title { get; set; } = "";
    public string? SubTitle { get; set; }
    public string? ImageUrl { get; set; }
}

public sealed class CategoryCardViewModel
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
}

public sealed class ProductCardViewModel
{
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public decimal Price { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? CategoryName { get; set; }
}

