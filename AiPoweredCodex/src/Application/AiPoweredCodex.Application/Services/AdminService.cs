using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Application.DTOs.Admin;
using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Application.Services;

public sealed class AdminService
{
    private readonly IUserRepository _userRepository;
    private readonly ICarRepository _carRepository;
    private readonly IBidRepository _bidRepository;

    public AdminService(IUserRepository userRepository, ICarRepository carRepository, IBidRepository bidRepository)
    {
        _userRepository = userRepository;
        _carRepository = carRepository;
        _bidRepository = bidRepository;
    }

    public async Task<AdminInsightsDto> GetInsightsAsync(CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllAsync(cancellationToken);
        var cars = await _carRepository.GetAllAsync(cancellationToken);
        var bids = await _bidRepository.GetAllAsync(cancellationToken);

        return new AdminInsightsDto(
            users.Count(x => x.Role == UserRole.User),
            users.Count(x => x.Role == UserRole.Seller),
            cars.Count,
            cars.Count(x => x.Status == CarStatus.Available),
            cars.Count(x => x.Status == CarStatus.Sold),
            bids.Count,
            bids.Count == 0 ? 0 : bids.Max(x => x.Amount));
    }
}
