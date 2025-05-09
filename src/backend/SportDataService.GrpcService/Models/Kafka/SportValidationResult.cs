namespace SportDataService.GrpcService.Models.Kafka;

public class SportValidationResult
{
    public string BetId { get; set; }
    public bool IsValid { get; set; }
    public string CorrelationId { get; set; }
    public string RejectionReason { get; set; }
    public double CurrentOdds { get; set; }
    public DateTime Timestamp { get; set; }
}