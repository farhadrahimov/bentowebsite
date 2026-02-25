namespace Tortcu.Domain;

public sealed class AboutContent
{
    public int Id { get; set; }
    public string Content { get; set; } = "";
    public string? MainImageUrl { get; set; }
}

