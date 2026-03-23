using System.Security.Claims;
using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Api.Extensions;

public static class HttpContextExtensions
{
    public static Guid? GetUserId(this HttpContext httpContext)
    {
        var value = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    public static string? GetUserRole(this HttpContext httpContext)
    {
        return httpContext.User.FindFirstValue(ClaimTypes.Role);
    }

    public static bool IsInRole(this HttpContext httpContext, UserRole role)
    {
        return string.Equals(httpContext.GetUserRole(), role.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}
