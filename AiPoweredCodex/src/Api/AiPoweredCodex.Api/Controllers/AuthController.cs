using AiPoweredCodex.Api.Extensions;
using AiPoweredCodex.Application.DTOs.Auth;
using AiPoweredCodex.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiPoweredCodex.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.RegisterAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return Unauthorized(new { message = exception.Message });
        }
    }

    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            return Unauthorized(new { message = "Bearer token is required." });
        }

        return Ok(new
        {
            userId,
            role = HttpContext.GetUserRole(),
            name = User.Identity?.Name,
            email = User.Claims.FirstOrDefault(x => x.Type.EndsWith("emailaddress", StringComparison.OrdinalIgnoreCase))?.Value
        });
    }
}
