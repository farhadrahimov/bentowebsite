namespace Tortcu.Domain;

public sealed class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public string ImageUrl { get; set; } = "";
    public bool IsPrimary { get; set; } = false;
    public int DisplayOrder { get; set; } = 0;
}

