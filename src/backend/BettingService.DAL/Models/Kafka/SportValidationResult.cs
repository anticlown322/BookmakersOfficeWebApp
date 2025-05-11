namespace BettingService.DAL.Models.Kafka;

public class SportValidationResult
{
    public string CorrelationId { get; set; }
    public bool IsValid { get; set; }
    public string RejectionReason { get; set; }
    public double CurrentOdds { get; set; }
}