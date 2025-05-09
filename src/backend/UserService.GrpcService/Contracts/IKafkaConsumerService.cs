using Confluent.Kafka;

namespace UserService.GrpcService.Contracts;

public interface IKafkaConsumerService
{
    IConsumer<string, string> CreateConsumer();
    void DisposeConsumer(IConsumer<string, string> consumer);
}