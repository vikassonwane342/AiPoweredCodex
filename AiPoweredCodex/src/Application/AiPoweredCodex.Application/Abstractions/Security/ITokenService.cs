using AiPoweredCodex.Domain.Entities;

namespace AiPoweredCodex.Application.Abstractions.Security;

public interface ITokenService
{
    TokenEnvelope GenerateToken(AppUser user);
    TokenValidationResult ValidateToken(string token);
}

public sealed record TokenEnvelope(string AccessToken, DateTime ExpiresAtUtc);

public sealed record AuthenticatedUser(Guid UserId, string FullName, string Email, string Role);

public sealed record TokenValidationResult(bool IsValid, AuthenticatedUser? User, DateTime? ExpiresAtUtc, string? Error);
