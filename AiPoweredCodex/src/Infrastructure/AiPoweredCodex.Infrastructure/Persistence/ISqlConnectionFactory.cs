using Microsoft.Data.SqlClient;

namespace AiPoweredCodex.Infrastructure.Persistence;

public interface ISqlConnectionFactory
{
    SqlConnection CreateConnection();
    SqlConnection CreateMasterConnection();
}
