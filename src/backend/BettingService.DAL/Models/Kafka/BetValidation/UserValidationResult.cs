namespace BettingService.DAL.Models.Kafka.BetValidation;

public class UserValidationResult
{
    public string BetId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? RejectionReason { get; set; }
    public double CurrentBalance { get; set; }
    public DateTime Timestamp { get; set; }
}