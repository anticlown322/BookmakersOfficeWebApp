namespace UserService.GrpcService.Models.Settings;

public class KafkaTopics
{
    public string BetValidation { get; set; }
    public string UserValidationResults { get; set; }
    public string BalanceUpdates { get; set; }
}