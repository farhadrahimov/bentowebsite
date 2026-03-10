using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;

namespace Tortcu.Infrastructure.Services;

public sealed class AdminAuthService : IAdminAuthService
{
    private readonly AppDbContext _db;

    public AdminAuthService(AppDbContext db) => _db = db;

    public async Task<(bool Success, string? Error)> ValidateAsync(string username, string password, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return (false, "İstifadəçi adı və şifrə tələb olunur.");

        var user = await _db.AdminUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Username == username.Trim(), ct);

        if (user is null)
            return (false, "İstifadəçi tapılmadı.");

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            return (false, "Şifrə yanlışdır.");

        return (true, null);
    }
}
