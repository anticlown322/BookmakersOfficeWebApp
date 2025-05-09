namespace UserService.GrpcService.Models.Settings;

public class KafkaTopics
{
    public string BetValidationRequests { get; set; }
    public string UserValidationResults { get; set; }
    public string PayoutRequests { get; set; }
    public string PayoutResults { get; set; }
}