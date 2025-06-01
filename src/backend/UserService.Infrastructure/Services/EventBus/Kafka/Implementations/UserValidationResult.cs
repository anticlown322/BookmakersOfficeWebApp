namespace UserService.Infrastructure.Services.EventBus.Kafka.Implementations;

public class UserValidationResult
{
    public string BetId { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? RejectionReason { get; set; }
    public double CurrentBalance { get; set; }
    public DateTime Timestamp { get; set; }
}