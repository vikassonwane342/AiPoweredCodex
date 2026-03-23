using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Domain.Entities;
using AiPoweredCodex.Domain.Enums;
using Microsoft.Data.SqlClient;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class SqlCarRepository : ICarRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SqlCarRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Car car, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Cars (Id, SellerId, Title, Brand, Model, [Year], StartingPrice, CurrentPrice, Description, Status, CreatedAtUtc)
            VALUES (@Id, @SellerId, @Title, @Brand, @Model, @Year, @StartingPrice, @CurrentPrice, @Description, @Status, @CreatedAtUtc);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", car.Id);
        command.Parameters.AddWithValue("@SellerId", car.SellerId);
        command.Parameters.AddWithValue("@Title", car.Title);
        command.Parameters.AddWithValue("@Brand", car.Brand);
        command.Parameters.AddWithValue("@Model", car.Model);
        command.Parameters.AddWithValue("@Year", car.Year);
        command.Parameters.AddWithValue("@StartingPrice", car.StartingPrice);
        command.Parameters.AddWithValue("@CurrentPrice", car.CurrentPrice);
        command.Parameters.AddWithValue("@Description", car.Description);
        command.Parameters.AddWithValue("@Status", (int)car.Status);
        command.Parameters.AddWithValue("@CreatedAtUtc", car.CreatedAtUtc);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Car>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, SellerId, Title, Brand, Model, [Year], StartingPrice, CurrentPrice, Description, Status, CreatedAtUtc
            FROM dbo.Cars
            ORDER BY CreatedAtUtc DESC;
            """;

        var cars = new List<Car>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            cars.Add(Map(reader));
        }

        return cars;
    }

    public async Task<Car?> GetByIdAsync(Guid carId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, SellerId, Title, Brand, Model, [Year], StartingPrice, CurrentPrice, Description, Status, CreatedAtUtc
            FROM dbo.Cars
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", carId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task UpdateAsync(Car car, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Cars
            SET CurrentPrice = @CurrentPrice,
                Status = @Status
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", car.Id);
        command.Parameters.AddWithValue("@CurrentPrice", car.CurrentPrice);
        command.Parameters.AddWithValue("@Status", (int)car.Status);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static Car Map(SqlDataReader reader)
    {
        return Car.Restore(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetString(2),
            reader.GetString(3),
            reader.GetString(4),
            reader.GetInt32(5),
            reader.GetDecimal(6),
            reader.GetDecimal(7),
            reader.GetString(8),
            (CarStatus)reader.GetInt32(9),
            reader.GetDateTime(10));
    }
}
