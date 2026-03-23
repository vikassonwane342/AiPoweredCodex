using AiPoweredCodex.Api.Extensions;
using AiPoweredCodex.Application.Services;
using AiPoweredCodex.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AiPoweredCodex.Api.Controllers;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    private readonly AdminService _adminService;

    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("insights")]
    public async Task<ActionResult> Insights(CancellationToken cancellationToken)
    {
        if (!HttpContext.IsInRole(UserRole.Admin))
        {
            return Unauthorized(new { message = "Admin bearer token is required." });
        }

        var insights = await _adminService.GetInsightsAsync(cancellationToken);
        return Ok(insights);
    }
}
