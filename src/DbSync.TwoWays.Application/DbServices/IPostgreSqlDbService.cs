using System.Data;

namespace DbSync.TwoWays.Application.DbServices;

public interface IPostgreSqlDbService
{
    Task<IDbConnection> GetOpenConnectionAsync(CancellationToken ct = default);
}
