using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace AiPoweredCodex.Infrastructure.Persistence;

public sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    public SqlConnectionFactory(IOptions<DatabaseOptions> options)
    {
        _connectionString = options.Value.DefaultConnection;
        if (string.IsNullOrWhiteSpace(_connectionString))
        {
            throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");
        }
    }

    public SqlConnection CreateConnection() => new(_connectionString);

    public SqlConnection CreateMasterConnection()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        };

        return new SqlConnection(builder.ConnectionString);
    }
}
