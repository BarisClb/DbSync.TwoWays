using DbSync.TwoWays.Application.DbServices;
using DbSync.TwoWays.Application.Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace DbSync.TwoWays.Application.Services;

// mssql: change tracking
// postgre: debezium

// mssql changetracking icin column track aktif edilecek. postgre tarafında debezium before ve after value'ları trackleyecek. 
// update işlemlerinde ekledigimiz 'updatedbysync' column:
// pg -> mssql: debezium before after valueları farklı ise skip
// mssql -> pg: sys_change_columns icerisinde ilgili column update edildigi belirtiliyor ise skip


// SORUN: changetracking, tek bir id donduruyor. aynı id icin birden cok işlem varsa, bunları tek satır donduruyor:
// 1 insert 2 update -> insert. 1 ve 2 column updatei, sonrasında 3 ve 4 column update -> 1,2,3,4 column updatedi
// eger sync custom column'u update ederse ve henuz bu degisiklik consume edilmediyse, bu aralıkta projelerden de degisiklik oldugunda, changetrackingten gelen deger iki update'i de icine alacak
// ilk updateden custom column da geldigi icin ignore edilecek


public sealed class SyncService3_MsSqlToPostgre : BaseSyncConsumer
{
    public SyncService3_MsSqlToPostgre(IMsSqlDbService msSql, IPostgreSqlDbService pg, IOptions<KafkaConsumerSettings> opt)
        : base(msSql, pg, opt.Value.BootstrapServers, opt.Value.Consumers["3_1"].GroupId, opt.Value.Consumers["3_1"].Topic)
    { }

    protected override async Task OnMessageAsync(string value, CancellationToken ct)
    {
        Console.WriteLine("[SyncService1] Message received");


        // get mssql changetracking id and columns info

        // check if updated columns list has custom column id
        // if it has it, skip


        using var msSqlConn = await MsSql.GetOpenConnectionAsync(ct);

        // get changes

        using var pgConn = await Pg.GetOpenConnectionAsync(ct);

        // apply changes

        Console.WriteLine($"[SyncService1] Message Processed.");
    }
}

public sealed class SyncService3_PostgreToMsSql : BaseSyncConsumer
{
    public SyncService3_PostgreToMsSql(IMsSqlDbService msSql, IPostgreSqlDbService pg, IOptions<KafkaConsumerSettings> opt)
        : base(msSql, pg, opt.Value.BootstrapServers, opt.Value.Consumers["3_2"].GroupId, opt.Value.Consumers["3_2"].Topic)
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
