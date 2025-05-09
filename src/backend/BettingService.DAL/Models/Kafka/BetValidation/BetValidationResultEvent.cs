namespace BettingService.DAL.Models.Kafka.BetValidation;

public class BetValidationResultEvent
{
    public string BetId { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string? RejectionReason { get; set; }
    public double CurrentOdds { get; set; }
    public DateTime Timestamp { get; set; }
}