using AiPoweredCodex.Domain.Entities;

namespace AiPoweredCodex.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task AddAsync(AppUser user, CancellationToken cancellationToken);
    Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
    Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken);
}
