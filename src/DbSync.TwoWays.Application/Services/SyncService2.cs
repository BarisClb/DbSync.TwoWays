using Dapper;
using DbSync.TwoWays.Application.DbServices;
using DbSync.TwoWays.Application.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DbSync.TwoWays.Application.Services;


// mssql: debezium
// postgre: debezium

// işlem update ise ve 'changes.Payload?' icerisinde before ve after 'updatedbysync' column valuesu farklı ise, sync projesindeki bir islemdir, skip.


public sealed class SyncService2_MsSqlToPostgre : BaseSyncConsumer
{
    public SyncService2_MsSqlToPostgre(IMsSqlDbService msSql, IPostgreSqlDbService pg, IOptions<KafkaConsumerSettings> opt)
        : base(msSql, pg, opt.Value.BootstrapServers, opt.Value.Consumers["2_1"].GroupId, opt.Value.Consumers["2_1"].Topic)
    { }

    protected override async Task OnMessageAsync(string value, CancellationToken ct)
    {
        Console.WriteLine("[SyncService1] Message received");

        var changes = JsonSerializer.Deserialize<DebeziumEnvelope<TestTable2>>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (changes?.Payload == null)
            return;

        if (string.Equals(changes.Payload?.Op, "u", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(changes.Payload?.Before?.TestColumn) && !string.Equals(changes.Payload.Before.TestColumn, changes.Payload.Before.TestColumn, StringComparison.OrdinalIgnoreCase))
                return;
        }

        using var msSqlConn = await MsSql.GetOpenConnectionAsync(ct);

        // get changes

        using var pgConn = await Pg.GetOpenConnectionAsync(ct);

        // apply changes

        Console.WriteLine($"[SyncService1] Message Processed.");
    }
}

public sealed class SyncService2_PostgreToMsSql : BaseSyncConsumer
{
    public SyncService2_PostgreToMsSql(IMsSqlDbService msSql, IPostgreSqlDbService pg, IOptions<KafkaConsumerSettings> opt)
        : base(msSql, pg, opt.Value.BootstrapServers, opt.Value.Consumers["2_2"].GroupId, opt.Value.Consumers["2_2"].Topic)
    { }

    protected override async Task OnMessageAsync(string value, CancellationToken ct)
    {
        Console.WriteLine("[SyncService1] Message received.");

        var changes = JsonSerializer.Deserialize<DebeziumEnvelope<TestTable2>>(value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (changes?.Payload == null)
            return;

        if (string.Equals(changes.Payload?.Op, "u", StringComparison.OrdinalIgnoreCase))
        {
            if (!string.IsNullOrEmpty(changes.Payload?.Before?.TestColumn) && !string.Equals(changes.Payload.Before.TestColumn, changes.Payload.Before.TestColumn, StringComparison.OrdinalIgnoreCase))
                return;
        }

        using var pgConn = await Pg.GetOpenConnectionAsync(ct);

        // get changes

        using var msSqlConn = await MsSql.GetOpenConnectionAsync(ct);
        
        // apply changes

        Console.WriteLine($"[SyncService1] Message Processed.");
    }
}
