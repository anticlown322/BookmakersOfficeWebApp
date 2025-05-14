namespace SportDataService.Infrastructure.Services.EventBus.Kafka.Abstractions;

public interface IEventProducer : IDisposable
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken ct = default)
        where T : class;

    Task ProduceAsync<T>(string topic, string key, T message, CancellationToken ct = default)
        where T : class;

    Task FlushAsync(TimeSpan timeout, CancellationToken ct = default);
}