namespace BettingService.DAL.Models.Settings.Kafka;

public class KafkaProducerConfig
{
    public string Acks { get; set; } = "All";
    public bool EnableIdempotence { get; set; } = true;
    public int MessageTimeoutMs { get; set; } = 5000;
    public int RetryBackoffMs { get; set; } = 100;
}