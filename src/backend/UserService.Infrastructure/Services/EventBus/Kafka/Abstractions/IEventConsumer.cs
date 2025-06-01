using UserService.Infrastructure.Services.EventBus.Kafka.Implementations;

namespace UserService.Infrastructure.Services.EventBus.Kafka.Abstractions;

public interface IEventConsumer<T> : IDisposable
    where T : class
{
    Task SubscribeAsync(string topic, CancellationToken ct = default);
    Task<ConsumeResult<T>> ConsumeAsync(CancellationToken ct);
    Task CommitAsync(ConsumeResult<T> result, CancellationToken ct = default);
}