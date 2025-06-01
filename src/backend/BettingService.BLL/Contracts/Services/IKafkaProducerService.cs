namespace BettingService.BLL.Contracts.Services;

public interface IKafkaProducerService
{
    Task ProduceAsync<T>(string topic, T message, CancellationToken cancellationToken);
}