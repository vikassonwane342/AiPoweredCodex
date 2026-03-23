using AiPoweredCodex.Domain.Entities;

namespace AiPoweredCodex.Application.Abstractions.Persistence;

public interface IBidRepository
{
    Task AddAsync(Bid bid, CancellationToken cancellationToken);
    Task<Bid?> GetByIdAsync(Guid bidId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Bid>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Bid>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Bid bid, CancellationToken cancellationToken);
}
