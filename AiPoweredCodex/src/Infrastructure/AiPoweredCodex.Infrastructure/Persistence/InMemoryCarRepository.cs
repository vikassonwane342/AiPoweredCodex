using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Domain.Entities;
using System.Collections.Concurrent;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class InMemoryCarRepository : ICarRepository
{
    private readonly ConcurrentDictionary<Guid, Car> _cars = new();

    public Task AddAsync(Car car, CancellationToken cancellationToken)
    {
        _cars[car.Id] = car;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken)
    {
        return Task.FromResult<IReadOnlyList<Car>>(_cars.Values.OrderByDescending(x => x.CreatedAtUtc).ToList());
    }

    public Task<Car?> GetByIdAsync(Guid carId, CancellationToken cancellationToken)
    {
        _cars.TryGetValue(carId, out var car);
        return Task.FromResult(car);
    }

    public Task UpdateAsync(Car car, CancellationToken cancellationToken)
    {
        _cars[car.Id] = car;
        return Task.CompletedTask;
    }
}
