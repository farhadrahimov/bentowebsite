namespace Tortcu.Domain;

public sealed class Campaign
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? SubTitle { get; set; }
    public string? ImageUrl { get; set; }

    public DateTime? StartDateUtc { get; set; }
    public DateTime? EndDateUtc { get; set; }

    public bool IsActive { get; set; } = false;
}

