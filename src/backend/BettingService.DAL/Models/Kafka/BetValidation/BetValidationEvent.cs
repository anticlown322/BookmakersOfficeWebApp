namespace BettingService.DAL.Models.Kafka.BetValidation;

public class BetValidationEvent
{
    public string BetId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string MatchId { get; set; } = string.Empty;
    public string LineType { get; set; } = string.Empty;
    public string MarketSelection { get; set; } = string.Empty;
    public double RequestedOdds { get; set; }
    public DateTime Timestamp { get; set; }
}