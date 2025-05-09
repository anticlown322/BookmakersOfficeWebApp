namespace BettingService.DAL.Models.Kafka;

public class PayoutResult
{
    public string PayoutId { get; set; }
    public string CorrelationId { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
    public decimal NewBalance { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}