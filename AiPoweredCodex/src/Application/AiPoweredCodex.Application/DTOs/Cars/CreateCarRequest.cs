namespace AiPoweredCodex.Application.DTOs.Cars;

public sealed record CreateCarRequest(string Title, string Brand, string Model, int Year, decimal StartingPrice, string Description);
