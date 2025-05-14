namespace SportDataService.Infrastructure.Services.EventBus.Kafka.Implementations;

public record ConsumeResult<T>(T Message, string Topic, int Partition, long Offset);