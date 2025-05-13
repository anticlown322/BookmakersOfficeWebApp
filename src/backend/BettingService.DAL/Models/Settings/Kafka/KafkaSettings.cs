namespace BettingService.DAL.Models.Settings.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = string.Empty;
    public int RequestTimeoutSeconds { get; set; }
    public int ConsumeMsgTimeoutSeconds { get; set; }
    public KafkaTopics Topics { get; set; } = new();
    public KafkaProducerConfig? ProducerConfig { get; set; }
}