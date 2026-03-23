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
            Console.WriteLine("AuthController.Register returning Ok.");
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("AuthController.Register returning BadRequest.");
            return BadRequest(new { message = exception.Message });
        }
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var response = await _authService.LoginAsync(request, cancellationToken);
            Console.WriteLine("AuthController.Login returning Ok.");
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            Console.WriteLine("AuthController.Login returning Unauthorized.");
            return Unauthorized(new { message = exception.Message });
        }
    }

    [HttpGet("me")]
    public ActionResult<object> Me()
    {
        var userId = HttpContext.GetUserId();
        if (userId is null)
        {
            Console.WriteLine("AuthController.Me returning Unauthorized.");
            return Unauthorized(new { message = "Bearer token is required." });
        }

        Console.WriteLine("AuthController.Me returning Ok.");
        return Ok(new
        {
            userId,
            role = HttpContext.GetUserRole(),
            name = User.Identity?.Name,
            email = User.Claims.FirstOrDefault(x => x.Type.EndsWith("emailaddress", StringComparison.OrdinalIgnoreCase))?.Value
        });
    }
}
