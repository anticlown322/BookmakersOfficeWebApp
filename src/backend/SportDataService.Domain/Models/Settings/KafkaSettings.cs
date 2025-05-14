namespace SportDataService.Domain.Models.Settings;

public class KafkaSettings
{
    public string BootstrapServers { get; set; }
    public string ConsumerGroup { get; set; }
    public int RequestTimeoutSeconds { get; set; }
    public int ConsumeMsgTimeoutSeconds { get; set; }
    public KafkaTopics Topics { get; set; } = new();
    public KafkaProducerConfig? ProducerConfig { get; set; }
}