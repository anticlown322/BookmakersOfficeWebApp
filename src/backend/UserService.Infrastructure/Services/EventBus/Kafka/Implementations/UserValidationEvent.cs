using System.Text.Json.Serialization;

namespace UserService.Infrastructure.Services.EventBus.Kafka.Implementations;

public class UserValidationEvent
{
    [JsonPropertyName("BetId")]
    public string BetId { get; set; } = string.Empty;

    [JsonPropertyName("CorrelationId")]
    public string CorrelationId { get; set; } = string.Empty;

    [JsonPropertyName("Username")]
    public string Username { get; set; } = string.Empty;

    [JsonPropertyName("Amount")]
    public decimal Amount { get; set; }

    [JsonPropertyName("Timestamp")]
    public DateTime Timestamp { get; set; }
}