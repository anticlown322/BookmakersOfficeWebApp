using Confluent.Kafka;

namespace BettingService.BLL.Contracts.Services;

public interface IKafkaConsumerService
{
    Task<T> ConsumeSingleMessageAsync<T>(string topic, TimeSpan timeout, CancellationToken cancellationToken);
}