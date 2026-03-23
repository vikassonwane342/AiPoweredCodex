using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Application.DTOs.Bids;
using AiPoweredCodex.Domain.Entities;
using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Application.Services;

public sealed class BidService
{
    private readonly ICarRepository _carRepository;
    private readonly IBidRepository _bidRepository;

    public BidService(ICarRepository carRepository, IBidRepository bidRepository)
    {
        _carRepository = carRepository;
        _bidRepository = bidRepository;
    }

    public async Task<BidDto> PlaceBidAsync(Guid buyerId, Guid carId, decimal amount, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new InvalidOperationException("Car not found.");

        if (car.Status is CarStatus.Sold or CarStatus.Cancelled or CarStatus.Paid)
        {
            throw new InvalidOperationException("This car is not open for bidding.");
        }

        car.RegisterBid(amount);
        await _carRepository.UpdateAsync(car, cancellationToken);

        var bid = new Bid(carId, buyerId, amount);
        await _bidRepository.AddAsync(bid, cancellationToken);

        return new BidDto(bid.Id, bid.CarId, bid.BuyerId, bid.Amount, bid.IsAccepted, bid.CreatedAtUtc);
    }

    public async Task<BidDto> DecideBidAsync(Guid sellerId, Guid carId, Guid bidId, bool accept, CancellationToken cancellationToken)
    {
        var car = await _carRepository.GetByIdAsync(carId, cancellationToken)
            ?? throw new InvalidOperationException("Car not found.");

        if (car.SellerId != sellerId)
        {
            throw new InvalidOperationException("Only the seller can decide bids for this car.");
        }

        var bid = await _bidRepository.GetByIdAsync(bidId, cancellationToken)
            ?? throw new InvalidOperationException("Bid not found.");

        if (bid.CarId != carId)
        {
            throw new InvalidOperationException("Bid does not belong to this car.");
        }

        bid.Decide(accept);
        await _bidRepository.UpdateAsync(bid, cancellationToken);

        car.UpdateStatus(accept ? CarStatus.Booked : CarStatus.Available);
        await _carRepository.UpdateAsync(car, cancellationToken);

        return new BidDto(bid.Id, bid.CarId, bid.BuyerId, bid.Amount, bid.IsAccepted, bid.CreatedAtUtc);
    }
}
