using AiPoweredCodex.Domain.Enums;

namespace AiPoweredCodex.Application.DTOs.Cars;

public sealed record CarDto(
    Guid Id,
    Guid SellerId,
    string Title,
    string Brand,
    string Model,
    int Year,
    decimal StartingPrice,
    decimal CurrentPrice,
    string Description,
    CarStatus Status,
    DateTime CreatedAtUtc);
