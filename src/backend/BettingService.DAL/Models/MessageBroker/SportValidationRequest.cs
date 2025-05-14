namespace BettingService.DAL.Models.MessageBroker;

public class SportValidationRequest
{
    public string BetId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string MatchId { get; set; }
    public string LineType { get; set; }
    public string MarketSelection { get; set; }
    public decimal RequestedOdds { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}