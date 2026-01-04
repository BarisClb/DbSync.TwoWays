using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace DbSync.TwoWays.Application.DbServices;

public class MsSqlDbService : IMsSqlDbService
{
    private readonly string _connectionString;

    public MsSqlDbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:SqlServer");
    }

    public async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct = default)
    {
        var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }
}
