using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Tortcu.Web.Areas.Cp.Controllers;

[Area("Cp")]
[Route("cp")]
[Authorize(Roles = "Admin")]
public sealed class DashboardController : Controller
{
    [HttpGet]
    [HttpGet("dashboard")]
    public IActionResult Index() => View();
}
