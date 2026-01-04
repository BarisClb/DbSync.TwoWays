using System.Text.Json.Serialization;

namespace DbSync.TwoWays.Application.Models;

public class DebeziumEnvelope<T>
{
    [JsonPropertyName("payload")]
    public DebeziumPayload<T>? Payload { get; set; }
}

public class DebeziumPayload<T>
{
    [JsonPropertyName("before")]
    public T? Before { get; set; }

    [JsonPropertyName("after")]
    public T? After { get; set; }

    // "op": c=create, u=update, d=delete, r=snapshot-read
    [JsonPropertyName("op")]
    public string? Op { get; set; }
}

