namespace BettingService.DAL.Models.Settings.Kafka;

public class KafkaTopics
{
    public string BetValidationRequests { get; set; }
    public string UserValidationResults { get; set; }
    public string SportValidationResults { get; set; }

    public string[] AllTopics => new[]
    {
        BetValidationRequests,
        UserValidationResults,
        SportValidationResults,
    };
}