namespace Tortcu.Infrastructure.Services;

public interface ISitemapService
{
    Task<string> GenerateXmlAsync(string baseUrl, CancellationToken ct);
}

