using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Application.DTOs.Auth;

public sealed record RegisterRequest(string FullName, string Email, string Password, UserRole Role);
