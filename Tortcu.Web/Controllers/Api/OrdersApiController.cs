using Microsoft.AspNetCore.Mvc;

namespace Tortcu.Web.Controllers.Api;

[ApiController]
[Route("api/orders")]
public sealed class OrdersApiController : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
        => StatusCode(StatusCodes.Status501NotImplemented, new
        {
            message = "Coming Soon"
        });
}

