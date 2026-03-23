using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Domain.Entities;

public sealed class Car
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid SellerId { get; init; }
    public string Title { get; private set; }
    public string Brand { get; private set; }
    public string Model { get; private set; }
    public int Year { get; private set; }
    public decimal StartingPrice { get; private set; }
    public decimal CurrentPrice { get; private set; }
    public string Description { get; private set; }
    public CarStatus Status { get; private set; } = CarStatus.Available;
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public Car(Guid sellerId, string title, string brand, string model, int year, decimal startingPrice, string description)
    {
        SellerId = sellerId;
        Title = title.Trim();
        Brand = brand.Trim();
        Model = model.Trim();
        Year = year;
        StartingPrice = startingPrice;
        CurrentPrice = startingPrice;
        Description = description.Trim();
    }

    public void RegisterBid(decimal amount)
    {
        if (amount <= CurrentPrice)
        {
            throw new InvalidOperationException("Bid amount must be higher than the current price.");
        }

        CurrentPrice = amount;
        Status = CarStatus.BidReceived;
    }

    public void UpdateStatus(CarStatus status)
    {
        Status = status;
    }
}
