namespace DbSync.TwoWays.Application.Models;

public class KafkaConsumerSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public Dictionary<string, ConsumerItem> Consumers { get; set; } = new();

    public class ConsumerItem
    {
        public string Topic { get; set; } = "";
        public string GroupId { get; set; } = "";
    }
}
