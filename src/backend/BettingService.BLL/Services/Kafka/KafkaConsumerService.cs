using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Models.Settings.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Kafka;

public class KafkaConsumerService(
    IOptions<KafkaSettings> settings,
    ILogger<KafkaConsumerService> logger)
    : IKafkaConsumerService, IDisposable
{
    private readonly KafkaSettings _settings = settings.Value;

    public IConsumer<string, string> CreateConsumer()
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = $"{_settings.ConsumerGroup}.sportdata",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            IsolationLevel = IsolationLevel.ReadCommitted,
        };

        return new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => logger.LogError($"Kafka consumer error: {e.Reason}"))
            .Build();
    }

    public void DisposeConsumer(IConsumer<string, string> consumer) => consumer?.Dispose();

    public void Dispose() { }
}