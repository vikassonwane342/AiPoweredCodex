using AiPoweredCodex.Application.Abstractions.Persistence;
using AiPoweredCodex.Domain.Entities;
using AiPoweredCodex.Domain.Enums;
using Microsoft.Data.SqlClient;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class SqlUserRepository : IUserRepository
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public SqlUserRepository(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task AddAsync(AppUser user, CancellationToken cancellationToken)
    {
        const string sql = """
            INSERT INTO dbo.AppUsers (Id, FullName, Email, PasswordHash, Role, CreatedAtUtc)
            VALUES (@Id, @FullName, @Email, @PasswordHash, @Role, @CreatedAtUtc);
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", user.Id);
        command.Parameters.AddWithValue("@FullName", user.FullName);
        command.Parameters.AddWithValue("@Email", user.Email);
        command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
        command.Parameters.AddWithValue("@Role", (int)user.Role);
        command.Parameters.AddWithValue("@CreatedAtUtc", user.CreatedAtUtc);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync(CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, FullName, Email, PasswordHash, Role, CreatedAtUtc
            FROM dbo.AppUsers
            ORDER BY CreatedAtUtc;
            """;

        var users = new List<AppUser>();
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        while (await reader.ReadAsync(cancellationToken))
        {
            users.Add(Map(reader));
        }

        return users;
    }

    public async Task<AppUser?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, FullName, Email, PasswordHash, Role, CreatedAtUtc
            FROM dbo.AppUsers
            WHERE Email = @Email;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Email", email.Trim().ToLowerInvariant());
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    public async Task<AppUser?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        const string sql = """
            SELECT Id, FullName, Email, PasswordHash, Role, CreatedAtUtc
            FROM dbo.AppUsers
            WHERE Id = @Id;
            """;

        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var command = new SqlCommand(sql, connection);
        command.Parameters.AddWithValue("@Id", userId);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        return await reader.ReadAsync(cancellationToken) ? Map(reader) : null;
    }

    private static AppUser Map(SqlDataReader reader)
    {
        return AppUser.Restore(
            reader.GetGuid(0),
            reader.GetString(1),
            reader.GetString(2),
            reader.GetString(3),
            (UserRole)reader.GetInt32(4),
            reader.GetDateTime(5));
    }
}
