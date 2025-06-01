namespace UserService.Infrastructure.Services.EventBus.Kafka.Implementations;

public class PayoutRequest
{
    public string PayoutId { get; set; } = Guid.NewGuid().ToString();
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string Username { get; set; }
    public decimal Amount { get; set; }
}