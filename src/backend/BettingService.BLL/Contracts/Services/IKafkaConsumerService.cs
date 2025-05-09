using Confluent.Kafka;

namespace BettingService.BLL.Contracts.Services;

public interface IKafkaConsumerService
{
    IConsumer<string, string> CreateConsumer();
    void DisposeConsumer(IConsumer<string, string> consumer);
}