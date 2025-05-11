namespace BettingService.DAL.Models.Kafka;

public class UserValidationResult
{
    public string CorrelationId { get; set; }
    public bool IsValid { get; set; }
    public string RejectionReason { get; set; }
}