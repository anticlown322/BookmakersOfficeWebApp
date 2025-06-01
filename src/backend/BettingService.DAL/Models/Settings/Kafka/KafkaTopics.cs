namespace BettingService.DAL.Models.Settings.Kafka;

public class KafkaTopics
{
    public string UserValidationRequests { get; set; }
    public string SportValidationRequests { get; set; }
    public string UserValidationResults { get; set; }
    public string SportValidationResults { get; set; }
    public string PayoutRequests { get; set; }
    public string PayoutResults { get; set; }
}