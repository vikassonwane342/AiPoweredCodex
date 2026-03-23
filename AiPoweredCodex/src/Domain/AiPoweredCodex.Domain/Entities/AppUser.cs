using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Domain.Entities;

public sealed class AppUser
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public UserRole Role { get; private set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public AppUser(string fullName, string email, string passwordHash, UserRole role)
    {
        FullName = fullName.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
        Role = role;
    }
}
