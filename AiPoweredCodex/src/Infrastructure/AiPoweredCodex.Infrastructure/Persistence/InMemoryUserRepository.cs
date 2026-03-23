using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Domain.Entities;
using System.Collections.Concurrent;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, AppUser> _users = new();

    public Task AddAsync(AppUser user, CancellationToken cancellationToken)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<AppUser>>(_users.Values.OrderBy(x => x.CreatedAtUtc).ToList());
    }

    public Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        var user = _users.Values.FirstOrDefault(x => x.Email.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        _users.TryGetValue(userId, out var user);
        return Task.FromResult(user);
    }
}
