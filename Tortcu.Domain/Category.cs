namespace Tortcu.Domain;

public sealed class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Slug { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; } = 0;

    public List<Product> Products { get; set; } = new();
}

