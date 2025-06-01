namespace BettingService.DAL.Models.MessageBroker;

public class SportValidationResult
{
    public string CorrelationId { get; set; }
    public bool IsValid { get; set; }
    public string RejectionReason { get; set; }
    public decimal CurrentOdds { get; set; }
}