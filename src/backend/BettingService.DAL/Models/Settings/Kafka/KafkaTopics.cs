namespace BettingService.DAL.Models.Settings.Kafka;

public class KafkaTopics
{
    public string BetValidation { get; set; }
    public string UserValidationResults { get; set; }
    public string SportValidationResults { get; set; }
    public string BalanceUpdates { get; set; }
    public string BetResults { get; set; }
    public string BetStatusUpdates { get; set; }

    public string[] AllTopics => new[]
    {
        BetValidation,
        UserValidationResults,
        SportValidationResults,
        BalanceUpdates,
        BetResults,
        BetStatusUpdates,
    };
}