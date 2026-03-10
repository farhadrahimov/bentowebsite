namespace Tortcu.Infrastructure.Services;

public interface IAdminAuthService
{
    Task<(bool Success, string? Error)> ValidateAsync(string username, string password, CancellationToken ct);
}
