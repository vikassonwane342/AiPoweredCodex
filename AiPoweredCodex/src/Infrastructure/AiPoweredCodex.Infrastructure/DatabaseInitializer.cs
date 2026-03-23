using AiPoweredCodex.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;

namespace AiPoweredCodex.Infrastructure;

public sealed class DatabaseInitializer
{
    private readonly ISqlConnectionFactory _connectionFactory;

    public DatabaseInitializer(ISqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync()
    {
        await EnsureDatabaseExistsAsync();
        await EnsureSchemaAsync();
    }

    private async Task EnsureDatabaseExistsAsync()
    {
        await using var connection = _connectionFactory.CreateMasterConnection();
        await connection.OpenAsync();

        const string sql = """
            IF DB_ID(N'carCodex') IS NULL
            BEGIN
                CREATE DATABASE [carCodex];
            END
            """;

        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private async Task EnsureSchemaAsync()
    {
        await using var connection = _connectionFactory.CreateConnection();
        await connection.OpenAsync();

        const string sql = """
            IF OBJECT_ID(N'dbo.AppUsers', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.AppUsers
                (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    FullName NVARCHAR(200) NOT NULL,
                    Email NVARCHAR(320) NOT NULL,
                    PasswordHash NVARCHAR(512) NOT NULL,
                    Role INT NOT NULL,
                    CreatedAtUtc DATETIME2 NOT NULL
                );

                CREATE UNIQUE INDEX IX_AppUsers_Email ON dbo.AppUsers(Email);
            END;

            IF OBJECT_ID(N'dbo.Cars', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Cars
                (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    SellerId UNIQUEIDENTIFIER NOT NULL,
                    Title NVARCHAR(200) NOT NULL,
                    Brand NVARCHAR(100) NOT NULL,
                    Model NVARCHAR(100) NOT NULL,
                    [Year] INT NOT NULL,
                    StartingPrice DECIMAL(18, 2) NOT NULL,
                    CurrentPrice DECIMAL(18, 2) NOT NULL,
                    Description NVARCHAR(2000) NOT NULL,
                    Status INT NOT NULL,
                    CreatedAtUtc DATETIME2 NOT NULL,
                    CONSTRAINT FK_Cars_AppUsers_SellerId FOREIGN KEY (SellerId) REFERENCES dbo.AppUsers(Id)
                );

                CREATE INDEX IX_Cars_SellerId ON dbo.Cars(SellerId);
            END;

            IF OBJECT_ID(N'dbo.Bids', N'U') IS NULL
            BEGIN
                CREATE TABLE dbo.Bids
                (
                    Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
                    CarId UNIQUEIDENTIFIER NOT NULL,
                    BuyerId UNIQUEIDENTIFIER NOT NULL,
                    Amount DECIMAL(18, 2) NOT NULL,
                    IsAccepted BIT NULL,
                    CreatedAtUtc DATETIME2 NOT NULL,
                    CONSTRAINT FK_Bids_Cars_CarId FOREIGN KEY (CarId) REFERENCES dbo.Cars(Id),
                    CONSTRAINT FK_Bids_AppUsers_BuyerId FOREIGN KEY (BuyerId) REFERENCES dbo.AppUsers(Id)
                );

                CREATE INDEX IX_Bids_CarId ON dbo.Bids(CarId);
                CREATE INDEX IX_Bids_BuyerId ON dbo.Bids(BuyerId);
            END;
            """;

        await using var command = new SqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }
}
