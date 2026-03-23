namespace AiPoweredCodex.Infrastructure;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "AiPoweredCodex";
    public string Audience { get; init; } = "AiPoweredCodex.Clients";
    public string SecretKey { get; init; } = "super-secret-development-key-change-me";
    public int AccessTokenMinutes { get; init; } = 60;
}
