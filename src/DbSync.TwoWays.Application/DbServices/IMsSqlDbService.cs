using System.Data;

namespace DbSync.TwoWays.Application.DbServices;

public interface IMsSqlDbService
{
    Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct = default);
}
