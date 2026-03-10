using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tortcu.Infrastructure.Data;

namespace Tortcu.Web.Areas.Cp.Controllers;

[Area("Cp")]
[Route("cp/campaigns")]
[Authorize(Roles = "Admin")]
public sealed class CampaignsController : Controller
{
    private const int TitleMaxLen = 200;
    private const int SubTitleMaxLen = 400;
    private const int MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public CampaignsController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    private async Task<string?> SaveImageAsync(IFormFile file, string subfolder, CancellationToken ct)
    {
        if (file.Length == 0 || file.Length > MaxFileSizeBytes) return null;
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (string.IsNullOrEmpty(ext) || !AllowedExtensions.Contains(ext)) return null;

        var uploadDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
        Directory.CreateDirectory(uploadDir);
        var fileName = $"{Guid.NewGuid():N}{ext}";
        var path = Path.Combine(uploadDir, fileName);
        await using var stream = new FileStream(path, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return $"/uploads/{subfolder}/{fileName}";
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var list = await _db.Campaigns.OrderByDescending(c => c.StartDateUtc ?? DateTime.MinValue).ToListAsync(ct);
        return View(list);
    }

    [HttpGet("create")]
    public IActionResult Create() => View();

    [HttpPost("create")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Create(
        [FromForm] string title,
        [FromForm] string? subTitle,
        [FromForm] string? imageUrl,
        [FromForm] IFormFile? imageFile,
        [FromForm] DateTime? startDateUtc,
        [FromForm] DateTime? endDateUtc,
        [FromForm] bool isActive,
        CancellationToken ct)
    {
        title = (title ?? "").Trim();
        if (string.IsNullOrEmpty(title))
        {
            ViewData["Error"] = "Başlıq boş ola bilməz.";
            return View();
        }
        if (title.Length > TitleMaxLen)
        {
            ViewData["Error"] = $"Başlıq max {TitleMaxLen} simvol ola bilər.";
            return View();
        }

        subTitle = (subTitle ?? "").Trim();
        if (subTitle.Length > SubTitleMaxLen)
        {
            ViewData["Error"] = $"Alt başlıq max {SubTitleMaxLen} simvol ola bilər.";
            return View();
        }

        var imgUrl = (string?)null;
        if (imageFile is { Length: > 0 })
            imgUrl = await SaveImageAsync(imageFile, "campaigns", ct);
        if (string.IsNullOrEmpty(imgUrl))
            imgUrl = (imageUrl ?? "").Trim();
        if (!string.IsNullOrEmpty(imgUrl) && imgUrl.Length > 600) imgUrl = imgUrl[..600];

        if (isActive)
        {
            await _db.Campaigns.Where(c => c.IsActive).ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, false), ct);
        }

        _db.Campaigns.Add(new Tortcu.Domain.Campaign
        {
            Title = title,
            SubTitle = string.IsNullOrEmpty(subTitle) ? null : subTitle,
            ImageUrl = string.IsNullOrEmpty(imgUrl) ? null : imgUrl,
            StartDateUtc = startDateUtc,
            EndDateUtc = endDateUtc,
            IsActive = isActive
        });
        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var c = await _db.Campaigns.FindAsync(new object[] { id }, ct);
        if (c == null) return NotFound();
        return View(c);
    }

    [HttpPost("edit/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxFileSizeBytes)]
    public async Task<IActionResult> Edit(
        int id,
        [FromForm] string title,
        [FromForm] string? subTitle,
        [FromForm] string? imageUrl,
        [FromForm] IFormFile? imageFile,
        [FromForm] DateTime? startDateUtc,
        [FromForm] DateTime? endDateUtc,
        [FromForm] bool isActive,
        CancellationToken ct)
    {
        var c = await _db.Campaigns.FindAsync(new object[] { id }, ct);
        if (c == null) return NotFound();

        title = (title ?? "").Trim();
        if (string.IsNullOrEmpty(title))
        {
            ViewData["Error"] = "Başlıq boş ola bilməz.";
            return View(c);
        }
        if (title.Length > TitleMaxLen)
        {
            ViewData["Error"] = $"Başlıq max {TitleMaxLen} simvol ola bilər.";
            return View(c);
        }

        subTitle = (subTitle ?? "").Trim();
        if (subTitle.Length > SubTitleMaxLen)
        {
            ViewData["Error"] = $"Alt başlıq max {SubTitleMaxLen} simvol ola bilər.";
            return View(c);
        }

        var imgUrl = (string?)null;
        if (imageFile is { Length: > 0 })
            imgUrl = await SaveImageAsync(imageFile, "campaigns", ct);
        if (string.IsNullOrEmpty(imgUrl))
            imgUrl = (imageUrl ?? "").Trim();
        if (!string.IsNullOrEmpty(imgUrl) && imgUrl.Length > 600) imgUrl = imgUrl[..600];

        if (isActive && !c.IsActive)
        {
            await _db.Campaigns.Where(x => x.IsActive && x.Id != id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false), ct);
        }

        c.Title = title;
        c.SubTitle = string.IsNullOrEmpty(subTitle) ? null : subTitle;
        c.ImageUrl = string.IsNullOrEmpty(imgUrl) ? null : imgUrl;
        c.StartDateUtc = startDateUtc;
        c.EndDateUtc = endDateUtc;
        c.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var c = await _db.Campaigns.FindAsync(new object[] { id }, ct);
        if (c == null) return NotFound();
        _db.Campaigns.Remove(c);
        await _db.SaveChangesAsync(ct);
        TempData["Success"] = "Kampaniya silindi.";
        return RedirectToAction(nameof(Index));
    }
}
