namespace AiPoweredCodex.Application.DTOs.Admin;

public sealed record AdminInsightsDto(
    int TotalUsers,
    int TotalSellers,
    int TotalCars,
    int AvailableCars,
    int SoldCars,
    int TotalBids,
    decimal HighestBidAmount);
