using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;

namespace Tortcu.Web.Areas.Cp.Controllers;

[Area("Cp")]
[Route("cp/about")]
[Authorize(Roles = "Admin")]
public sealed class AboutController : Controller
{
    private const int ContentMaxLen = 15_000; // mətn limiti
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public AboutController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    private async Task<string?> SaveImageAsync(IFormFile file, CancellationToken ct)
    {
        if (file.Length == 0 || file.Length > MaxFileSizeBytes) return null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext)) return null;

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", "about");
        Directory.CreateDirectory(uploadDir);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(uploadDir, fileName);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return $"/uploads/about/{fileName}";
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var content = await _db.AboutContents.OrderByDescending(x => x.Id).FirstOrDefaultAsync(ct);
        if (content == null)
        {
            content = new Tortcu.Domain.AboutContent { Content = "", MainImageUrl = null };
        }
        ViewData["ContentMaxLen"] = ContentMaxLen;
        return View(content);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Index(
        [FromForm] string content,
        [FromForm] string? mainImageUrl,
        [FromForm] IFormFile? mainImageFile,
        CancellationToken ct)
    {
        content = content ?? "";
        if (content.Length > ContentMaxLen)
        {
            ViewData["Error"] = $"Mətn max {ContentMaxLen:N0} simvol ola bilər.";
            ViewData["ContentMaxLen"] = ContentMaxLen;
            return View(new Tortcu.Domain.AboutContent { Content = content, MainImageUrl = (mainImageUrl ?? "").Trim() });
        }

        var imgUrl = (string?)null;
        if (mainImageFile is { Length: > 0 })
            imgUrl = await SaveImageAsync(mainImageFile, ct);
        if (string.IsNullOrEmpty(imgUrl))
            imgUrl = (mainImageUrl ?? "").Trim();
        if (!string.IsNullOrEmpty(imgUrl) && imgUrl.Length > 600) imgUrl = imgUrl[..600];

        var entity = await _db.AboutContents.OrderByDescending(x => x.Id).FirstOrDefaultAsync(ct);
        if (entity == null)
        {
            entity = new Tortcu.Domain.AboutContent { Content = content };
            _db.AboutContents.Add(entity);
        }
        else
        {
            entity.Content = content;
        }
        entity.MainImageUrl = string.IsNullOrEmpty(imgUrl) ? null : imgUrl;
        await _db.SaveChangesAsync(ct);

        TempData["Success"] = "Haqqımızda səhifəsi yadda saxlanıldı.";
        return RedirectToAction(nameof(Index));
    }
}
