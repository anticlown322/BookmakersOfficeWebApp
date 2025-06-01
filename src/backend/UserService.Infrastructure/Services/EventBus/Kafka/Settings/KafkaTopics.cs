namespace UserService.Infrastructure.Services.EventBus.Kafka.Settings;

public class KafkaTopics
{
    public string UserValidationRequests { get; set; }
    public string UserValidationResults { get; set; }
    public string PayoutRequests { get; set; }
    public string PayoutResults { get; set; }
}