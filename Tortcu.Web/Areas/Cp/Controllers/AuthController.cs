using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tortcu.Infrastructure.Services;

namespace Tortcu.Web.Areas.Cp.Controllers;

[Area("Cp")]
[Route("cp/auth")]
public sealed class AuthController : Controller
{
    private readonly IAdminAuthService _auth;

    public AuthController(IAdminAuthService auth) => _auth = auth;

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction(nameof(DashboardController.Index), "Dashboard", new { area = "Cp" });
        return View("Login");
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("login")]
    public async Task<IActionResult> Index([FromForm] string username, [FromForm] string password, CancellationToken ct)
    {
        var (success, error) = await _auth.ValidateAsync(username ?? "", password ?? "", ct);

        if (!success)
        {
            ViewData["Error"] = error;
            return View("Login");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username!.Trim()),
            new(ClaimTypes.Role, "Admin")
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            });

        return RedirectToAction(nameof(DashboardController.Index), "Dashboard", new { area = "Cp" });
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Index));
    }
}
