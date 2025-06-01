namespace SportDataService.Infrastructure.Services.EventBus.Kafka.Implementations;

public class SportValidationEvent
{
    public string BetId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public string MatchId { get; set; } = string.Empty;
    public string LineType { get; set; } = string.Empty;
    public string MarketSelection { get; set; } = string.Empty;
    public double RequestedOdds { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}