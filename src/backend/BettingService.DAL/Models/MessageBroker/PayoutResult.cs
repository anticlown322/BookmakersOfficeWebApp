namespace BettingService.DAL.Models.MessageBroker;

public class PayoutResult
{
    public string CorrelationId { get; set; }
    public bool IsSuccess { get; set; }
    public string ErrorMessage { get; set; }
}