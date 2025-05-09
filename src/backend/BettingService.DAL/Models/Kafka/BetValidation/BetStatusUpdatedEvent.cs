namespace BettingService.DAL.Models.Kafka.BetValidation;

public class BetStatusUpdatedEvent
{
    public string BetId { get; set; }
    public string NewStatus { get; set; }
    public double? UpdatedOdds { get; set; }
    public DateTime Timestamp { get; set; }
}