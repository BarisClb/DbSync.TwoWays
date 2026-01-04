using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Data;

namespace DbSync.TwoWays.Application.DbServices;

public class PostgreSqlDbService : IPostgreSqlDbService
{
    private readonly string _connectionString;

    public PostgreSqlDbService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:Postgres");
    }

    public async Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct = default)
    {
        var conn = new NpgsqlConnection(_connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }
}