namespace BettingService.DAL.Models.Settings.Kafka;

public class KafkaSettings
{
    public string BootstrapServers { get; set; } = string.Empty;
    public string ConsumerGroup { get; set; } = string.Empty;
    public string SecurityProtocol { get; set; } = string.Empty;
    public KafkaTopics Topics { get; set; } = new();
    public KafkaProducerConfig? ProducerConfig { get; set; }
}