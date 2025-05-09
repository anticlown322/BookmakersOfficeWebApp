namespace BettingService.DAL.Models.Kafka;

public class BetValidationRequest
{
    public string BetId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string ValidationType { get; set; }

    public string Username { get; set; }
    public double Amount { get; set; }

    public string MatchId { get; set; }
    public string LineType { get; set; }
    public string MarketSelection { get; set; }
    public double RequestedOdds { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}