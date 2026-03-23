using System.Security.Claims;
using AiPoweredCodex.Application.Abstractions.Security;

namespace AiPoweredCodex.Api.Middleware;

public sealed class TokenAuthenticationMiddleware
{
    private readonly RequestDelegate _next;

    public TokenAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITokenService tokenService)
    {
        var authorizationHeader = context.Request.Headers.Authorization.ToString();
        if (authorizationHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = authorizationHeader[7..].Trim();
            var validation = tokenService.ValidateToken(token);
            if (validation.IsValid && validation.User is not null)
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, validation.User.UserId.ToString()),
                    new Claim(ClaimTypes.Name, validation.User.FullName),
                    new Claim(ClaimTypes.Email, validation.User.Email),
                    new Claim(ClaimTypes.Role, validation.User.Role)
                };

                context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
            }
        }

        await _next(context);
    }
}
