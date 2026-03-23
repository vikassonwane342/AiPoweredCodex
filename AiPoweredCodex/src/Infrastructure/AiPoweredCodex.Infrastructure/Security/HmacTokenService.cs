using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using AiPoweredCodex.Application.Abstractions.Security;
using AiPoweredCodex.Domain.Entities;
using Microsoft.Extensions.Options;

namespace AiPoweredCodex.Infrastructure.Security;

public sealed class HmacTokenService : ITokenService
{
    private readonly JwtOptions _options;

    public HmacTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public TokenEnvelope GenerateToken(AppUser user)
    {
        var expiresAtUtc = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);
        var header = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new { alg = "HS256", typ = "JWT" }));
        var payload = Base64UrlEncode(JsonSerializer.SerializeToUtf8Bytes(new
        {
            sub = user.Id,
            name = user.FullName,
            email = user.Email,
            role = user.Role.ToString(),
            iss = _options.Issuer,
            aud = _options.Audience,
            exp = new DateTimeOffset(expiresAtUtc).ToUnixTimeSeconds()
        }));

        var signature = ComputeSignature($"{header}.{payload}");
        return new TokenEnvelope($"{header}.{payload}.{signature}", expiresAtUtc);
    }

    public TokenValidationResult ValidateToken(string token)
    {
        var parts = token.Split('.');
        if (parts.Length != 3)
        {
            return new TokenValidationResult(false, null, null, "Token format is invalid.");
        }

        var expectedSignature = ComputeSignature($"{parts[0]}.{parts[1]}");
        if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(expectedSignature), Encoding.UTF8.GetBytes(parts[2])))
        {
            return new TokenValidationResult(false, null, null, "Token signature is invalid.");
        }

        JwtPayload? payload;
        try
        {
            payload = JsonSerializer.Deserialize<JwtPayload>(Base64UrlDecode(parts[1]));
        }
        catch
        {
            return new TokenValidationResult(false, null, null, "Token payload is invalid.");
        }

        if (payload is null)
        {
            return new TokenValidationResult(false, null, null, "Token payload is missing.");
        }

        if (!string.Equals(payload.iss, _options.Issuer, StringComparison.Ordinal) ||
            !string.Equals(payload.aud, _options.Audience, StringComparison.Ordinal))
        {
            return new TokenValidationResult(false, null, null, "Token issuer or audience is invalid.");
        }

        var expiresAtUtc = DateTimeOffset.FromUnixTimeSeconds(payload.exp).UtcDateTime;
        if (expiresAtUtc <= DateTime.UtcNow)
        {
            return new TokenValidationResult(false, null, expiresAtUtc, "Token has expired.");
        }

        return new TokenValidationResult(
            true,
            new AuthenticatedUser(payload.sub, payload.name, payload.email, payload.role),
            expiresAtUtc,
            null);
    }

    private string ComputeSignature(string value)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.SecretKey));
        var signature = hmac.ComputeHash(Encoding.UTF8.GetBytes(value));
        return Base64UrlEncode(signature);
    }

    private static string Base64UrlEncode(byte[] input)
    {
        return Convert.ToBase64String(input).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    }

    private static byte[] Base64UrlDecode(string input)
    {
        var padded = input.Replace('-', '+').Replace('_', '/');
        padded = padded.PadRight(padded.Length + ((4 - padded.Length % 4) % 4), '=');
        return Convert.FromBase64String(padded);
    }

    private sealed record JwtPayload(Guid sub, string name, string email, string role, string iss, string aud, long exp);
}
