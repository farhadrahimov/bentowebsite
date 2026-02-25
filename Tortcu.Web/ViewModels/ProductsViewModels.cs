namespace Tortcu.Web.ViewModels;

public sealed class ProductsIndexViewModel
{
    public string? CategoryName { get; set; }
    public string? CategorySlug { get; set; }
    public List<CategoryCardViewModel> Categories { get; set; } = new();
    public List<ProductCardViewModel> Products { get; set; } = new();
}

public sealed class ProductDetailViewModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? CategoryName { get; set; }
    public List<ProductImageViewModel> Images { get; set; } = new();
}

public sealed class ProductImageViewModel
{
    public string Url { get; set; } = "";
    public bool IsPrimary { get; set; }
}

