using DbSync.TwoWays.Application.DbServices;
using DbSync.TwoWays.Application.Models;
using Microsoft.Extensions.Options;

namespace DbSync.TwoWays.Application.Services;


// ai suggestion:

/*
 * 
Required columns (minimum)

sync_origin (who last wrote: MSSQL or PG)
sync_event_id (uuid) (for idempotency/dedup)
sync_updated_at (optional but helps debugging)

DB-level enforcement (important)

Postgres trigger: if writer is NOT pg_datasync, set sync_origin = PG.
SQL Server trigger: if writer is NOT mssql_datasync, set sync_origin = MSSQL.

Then your rule becomes dead simple:

PG→MSSQL consumer: ignore events where sync_origin == MSSQL
MSSQL→PG publisher/consumer: ignore rows where sync_origin == PG


----

PG Debezium event has LSN + txId + table + PK (unique-ish per change stream position)
MSSQL CT event has (table + PK + SYS_CHANGE_VERSION + operation) (unique-ish)
Build a unified internal ChangeId:
For PG: pg:{topic}:{partition}:{offset} (or LSN) is enough
For MSSQL: mssql:{table}:{pk}:{change_version}:{op}
When you apply a change to the other DB, you record ChangeId in sync_applied_log
When you see a change coming from the other side, you check the log:
If already applied → skip
Else apply → log it

*/


public sealed class SyncService4 : BaseSyncConsumer
{
    public SyncService4(IMsSqlDbService msSql, IPostgreSqlDbService pg, IOptions<KafkaConsumerSettings> opt)
        : base(msSql, pg, opt.Value.BootstrapServers, opt.Value.Consumers["4"].GroupId, opt.Value.Consumers["4"].Topic)
    { }
}
