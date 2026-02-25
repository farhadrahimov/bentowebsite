namespace Tortcu.Domain;

public sealed class Product
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public string? Description { get; set; }
    public decimal Price { get; set; }

    public bool IsActive { get; set; } = true;
    public bool IsPopular { get; set; } = false;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public List<ProductImage> Images { get; set; } = new();
}

