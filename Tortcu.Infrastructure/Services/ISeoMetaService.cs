using Tortcu.Infrastructure.ViewModels;

namespace Tortcu.Infrastructure.Services;

public interface ISeoMetaService
{
    Task<SeoMetaViewModel> GetForPageAsync(string pageType, int? entityId, string? canonicalUrl, CancellationToken ct);
}

