using System.Text.Json.Serialization;

namespace DbSync.TwoWays.Application.Models;

public class TestTable4
{
    public int? Id { get; set; }
    public int? IntColumn { get; set; }
    public string? TextColumn { get; set; }
    public Guid? GuidColumn { get; set; }
    public DateTime? DateColumn { get; set; }
    public string? TestColumn { get; set; }

    [JsonPropertyName("sync_origin")]
    public short? SyncOrigin { get; set; }
    [JsonPropertyName("sync_event_id")]
    public Guid? SyncEventId { get; set; }
    [JsonPropertyName("sync_updated_at")]
    public DateTime? SyncUpdatedAt { get; set; }
}
