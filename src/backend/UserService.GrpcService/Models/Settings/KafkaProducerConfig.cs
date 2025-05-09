namespace UserService.GrpcService.Models.Settings;

public class KafkaProducerConfig
{
    public string Acks { get; set; } = "All";
    public bool EnableIdempotence { get; set; } = true;
    public int MessageTimeoutMs { get; set; } = 15000;
    public int RequestTimeoutMs { get; set; } = 10000;
    public int SocketTimeoutMs { get; set; } = 10000;
    public int RetryBackoffMs { get; set; } = 500;
    public int MessageSendMaxRetries { get; set; } = 5;
    public int LingerMs { get; set; } = 5;
    public int BatchSizeKB { get; set; } = 32;
}