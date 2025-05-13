namespace BettingService.DAL.Models.MessageBroker;

public class PayoutRequest
{
    public string PayoutId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string BetId { get; set; }
    public string Username { get; set; }
    public decimal Amount { get; set; }
}