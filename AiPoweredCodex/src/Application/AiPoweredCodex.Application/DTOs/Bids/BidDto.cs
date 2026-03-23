namespace AiPoweredCodex.Application.DTOs.Bids;

public sealed record BidDto(Guid Id, Guid CarId, Guid BuyerId, decimal Amount, bool? IsAccepted, DateTime CreatedAtUtc);
