using DbSync.TwoWays.Application.DbServices;
using DbSync.TwoWays.Application.Models;
using Microsoft.Extensions.Options;
using Dapper;


namespace DbSync.TwoWays.Application.Services;

public sealed class SyncService1 : BaseSyncConsumer
{
    public SyncService1(IMsSqlDbService msSql, IPostgreSqlDbService pg, IOptions<KafkaConsumerSettings> opt)
        : base(msSql, pg, opt.Value.BootstrapServers, opt.Value.Consumers["1"].GroupId, opt.Value.Consumers["1"].Topic)
    { }

    protected override async Task OnMessageAsync(string value, CancellationToken ct)
    {
        Console.WriteLine("[SyncService1] Message received");

        using var msSqlConn = await MsSql.GetOpenConnectionAsync(ct);
        await msSqlConn.ExecuteAsync("");

        var mssqlCount = await msSqlConn.ExecuteScalarAsync<int>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM dbo.test_table_4",
                cancellationToken: ct));

        using var pgConn = await Pg.GetOpenConnectionAsync(ct);
        var pgCount = await pgConn.ExecuteScalarAsync<int>(
            new CommandDefinition(
                "SELECT COUNT(1) FROM test_table_4",
                cancellationToken: ct));

        Console.WriteLine($"[SyncService1] MSSQL={mssqlCount}, PG={pgCount}");
    }
}
