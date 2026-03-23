namespace AiPoweredCodex.Domain.Entities;

public sealed class Bid
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid CarId { get; init; }
    public Guid BuyerId { get; init; }
    public decimal Amount { get; init; }
    public bool? IsAccepted { get; private set; }
    public DateTime CreatedAtUtc { get; init; } = DateTime.UtcNow;

    public Bid(Guid carId, Guid buyerId, decimal amount)
    {
        CarId = carId;
        BuyerId = buyerId;
        Amount = amount;
    }

    public static Bid Restore(Guid id, Guid carId, Guid buyerId, decimal amount, bool? isAccepted, DateTime createdAtUtc)
    {
        return new Bid(carId, buyerId, amount)
        {
            Id = id,
            IsAccepted = isAccepted,
            CreatedAtUtc = createdAtUtc
        };
    }

    public void Decide(bool accept)
    {
        IsAccepted = accept;
    }
}
