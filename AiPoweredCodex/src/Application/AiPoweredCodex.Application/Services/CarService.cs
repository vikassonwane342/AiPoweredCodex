using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Application.DTOs.Bids;
using AiPoweredCodex.Application.DTOs.Cars;
using AiPoweredCodex.Domain.Entities;
using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Application.Services;

public sealed class CarService
{
    private readonly ICarRepository _carRepository;
    private readonly IBidRepository _bidRepository;

    public CarService(ICarRepository carRepository, IBidRepository bidRepository)
    {
        _carRepository = carRepository;
        _bidRepository = bidRepository;
    }

    public async Task<IReadOnlyList<CarDto>> GetCarsAsync(CancellationToken cancellationToken)
    {
        var cars = await _carRepository.GetAllAsync(cancellationToken);
        return cars.OrderByDescending(x => x.CreatedAtUtc).Select(Map).ToList();
    }

    public async Task<CarDto?> GetCarByIdAsync(Guid carId, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByIdAsync(carId, cancellationToken);
        return car is null ? null : Map(car);
    }

    public async Task<CarDto> CreateCarAsync(Guid sellerId, CreateCarRequest request, CancellationToken cancellationToken)
    {
        if (request.StartingPrice <= 0)
        {
            throw new InvalidOperationException("Starting price must be greater than zero.");
        }

        var car = new Car(sellerId, request.Title, request.Brand, request.Model, request.Year, request.StartingPrice, request.Description);
        await _carRepository.AddAsync(car, cancellationToken);
        return Map(car);
    }

    public async Task<CarDto> UpdateStatusAsync(Guid actorId, string role, Guid carId, CarStatus status, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new InvalidOperationException("Car not found.");

        var isAdmin = string.Equals(role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && car.SellerId != actorId)
        {
            throw new InvalidOperationException("Only the seller or admin can update the car status.");
        }

        car.UpdateStatus(status);
        await _carRepository.UpdateAsync(car, cancellationToken);
        return Map(car);
    }

    public async Task<IReadOnlyList<BidDto>> GetBidsAsync(Guid actorId, string role, Guid carId, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new InvalidOperationException("Car not found.");

        var isAdmin = string.Equals(role, UserRole.Admin.ToString(), StringComparison.OrdinalIgnoreCase);
        if (!isAdmin && car.SellerId != actorId)
        {
            throw new InvalidOperationException("Only the seller or admin can view bids for this car.");
        }

        var bids = await _bidRepository.GetByCarIdAsync(carId, cancellationToken);
        return bids.OrderByDescending(x => x.Amount)
            .Select(x => new BidDto(x.Id, x.CarId, x.BuyerId, x.Amount, x.IsAccepted, x.CreatedAtUtc))
            .ToList();
    }

    private static CarDto Map(Car car) => new(
        car.Id,
        car.SellerId,
        car.Title,
        car.Brand,
        car.Model,
        car.Year,
        car.StartingPrice,
        car.CurrentPrice,
        car.Description,
        car.Status,
        car.CreatedAtUtc);
}
