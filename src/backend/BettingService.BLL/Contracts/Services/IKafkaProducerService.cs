namespace BettingService.BLL.Contracts.Services;

public interface IKafkaProducerService
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken ct = default);
    Task ProduceAsync<T>(string topic, string key, T message, CancellationToken ct = default);
    void Flush(TimeSpan timeout);
}