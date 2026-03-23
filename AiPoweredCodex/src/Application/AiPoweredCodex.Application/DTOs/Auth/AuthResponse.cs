namespace AiPoweredCodex.Application.DTOs.Auth;

public sealed record AuthResponse(string AccessToken, DateTime ExpiresAtUtc, UserDto User);
