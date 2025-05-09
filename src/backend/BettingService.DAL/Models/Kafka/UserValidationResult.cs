namespace BettingService.DAL.Models.Kafka;

public class UserValidationResult
{
    public string BetId { get; set; }
    public string CorrelationId { get; set; }
    public string ValidationType { get; set; } = "User";
    public bool IsValid { get; set; }
    public string RejectionReason { get; set; }
    public double CurrentBalance { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}