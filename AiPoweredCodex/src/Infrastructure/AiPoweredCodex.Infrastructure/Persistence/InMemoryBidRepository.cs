using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Domain.Entities;
using System.Collections.Concurrent;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class InMemoryBidRepository : IBidRepository
{
    private readonly ConcurrentDictionary<Guid, Bid> _bids = new();

    public Task AddAsync(Bid bid, CancellationToken cancellationToken)
    {
        _bids[bid.Id] = bid;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Bid>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Bid>>(_bids.Values.OrderByDescending(x => x.CreatedAtUtc).ToList());
    }

    public Task<IReadOnlyList<Bid>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Bid>>(_bids.Values.Where(x => x.CarId == carId).OrderByDescending(x => x.Amount).ToList());
    }

    public Task<Bid?> GetByIdAsync(Guid bidId, CancellationToken cancellationToken)
    {
        _bids.TryGetValue(bidId, out var bid);
        return Task.FromResult(bid);
    }

    public Task UpdateAsync(Bid bid, CancellationToken cancellationToken)
    {
        _bids[bid.Id] = bid;
        return Task.CompletedTask;
    }
}
