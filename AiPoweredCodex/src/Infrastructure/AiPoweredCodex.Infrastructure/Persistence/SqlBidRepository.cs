using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Domain.Entities;
using Microsoft.Data.SqlClient;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class SqlBidRepository : IBidRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SqlBidRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(Bid bid, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.Bids (Id, CarId, BuyerId, Amount, IsAccepted, CreatedAtUtc)
            VALUES (@Id, @CarId, @BuyerId, @Amount, @IsAccepted, @CreatedAtUtc);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", bid.Id);
        command.Parameters.AddWithValue("@CarId", bid.CarId);
        command.Parameters.AddWithValue("@BuyerId", bid.BuyerId);
        command.Parameters.AddWithValue("@Amount", bid.Amount);
        command.Parameters.AddWithValue("@IsAccepted", bid.IsAccepted is null ? DBNull.Value : bid.IsAccepted);
        command.Parameters.AddWithValue("@CreatedAtUtc", bid.CreatedAtUtc);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Bid>> GetByCarIdAsync(Guid carId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, CarId, BuyerId, Amount, IsAccepted, CreatedAtUtc
            FROM dbo.Bids
            WHERE CarId = @CarId
            ORDER BY Amount DESC;
            """;

        var bids = new List<Bid>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@CarId", carId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            bids.Add(Map(reader));
        }

        return bids;
    }

    public async Task<IReadOnlyList<Bid>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, CarId, BuyerId, Amount, IsAccepted, CreatedAtUtc
            FROM dbo.Bids
            ORDER BY CreatedAtUtc DESC;
            """;

        var bids = new List<Bid>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            bids.Add(Map(reader));
        }

        return bids;
    }

    public async Task<Bid?> GetByIdAsync(Guid bidId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, CarId, BuyerId, Amount, IsAccepted, CreatedAtUtc
            FROM dbo.Bids
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", bidId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task UpdateAsync(Bid bid, CancellationToken cancellationToken)
    {
        const string sql = """
            UPDATE dbo.Bids
            SET IsAccepted = @IsAccepted
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", bid.Id);
        command.Parameters.AddWithValue("@IsAccepted", bid.IsAccepted is null ? DBNull.Value : bid.IsAccepted);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    private static Bid Map(SqlDataReader reader)
    {
        return Bid.Restore(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetGuid(2),
            reader.GetDecimal(3),
            reader.IsDBNull(4) ? null : reader.GetBoolean(4),
            reader.GetDateTime(5));
    }
}
