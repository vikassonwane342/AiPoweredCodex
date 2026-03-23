using AiPoweredCodex.Domain.Entities;

namespace AiPoweredCodex.Application.Abstractions.Persistence;

public interface ICarRepository
{
    Task AddAsync(Car car, CancellationToken cancellationToken);
    Task<Car?> GetByIdAsync(Guid carId, CancellationToken cancellationToken);
    Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateAsync(Car car, CancellationToken cancellationToken);
}
