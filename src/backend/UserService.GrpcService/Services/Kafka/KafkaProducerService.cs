using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using UserService.GrpcService.Contracts;
using UserService.GrpcService.Models.Settings;

namespace UserService.GrpcService.Services.Kafka;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private readonly KafkaSettings _settings;

    public KafkaProducerService(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaProducerService> logger)
    {
        _settings = settings.Value;
        _logger = logger;

        var producerConfig = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = _settings.ProducerConfig.Acks switch
            {
                "All" => Acks.All,
                "Leader" => Acks.Leader,
                _ => Acks.None,
            },
            EnableIdempotence = _settings.ProducerConfig.EnableIdempotence,
            MessageTimeoutMs = _settings.ProducerConfig.MessageTimeoutMs,
            RequestTimeoutMs = _settings.ProducerConfig.RequestTimeoutMs,
            SocketTimeoutMs = _settings.ProducerConfig.SocketTimeoutMs,
            RetryBackoffMs = _settings.ProducerConfig.RetryBackoffMs,
            MessageSendMaxRetries = _settings.ProducerConfig.MessageSendMaxRetries,
            LingerMs = _settings.ProducerConfig.LingerMs,
            BatchSize = _settings.ProducerConfig.BatchSizeKB * 1024,
            QueueBufferingMaxMessages = 100_000,
        };

        _producer = new ProducerBuilder<string, string>(producerConfig)
            .SetLogHandler((_, logMessage) =>
                _logger.Log(ToLogLevel(logMessage.Level), logMessage.Message))
            .Build();
    }

    public async Task ProduceAsync<T>(string topic, T message, CancellationToken ct = default)
        => await ProduceAsync(topic, null!, message, ct);

    public async Task ProduceAsync<T>(string topic, string key, T message, CancellationToken ct = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(message);
            var kafkaMessage = new Message<string, string>
            {
                Key = key,
                Value = json
            };

            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, ct);

            _logger.LogDebug(
                "Delivered message to {Topic} [Partition: {Partition}, Offset: {Offset}]",
                deliveryResult.Topic,
                deliveryResult.Partition,
                deliveryResult.Offset);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(
                ex,
                "Failed to deliver message to {Topic}: {Reason}",
                topic,
                ex.Error.Reason);
            throw;
        }
    }

    public void Flush(TimeSpan timeout)
        => _producer.Flush(timeout);

    public void Dispose()
    {
        try
        {
            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Error during Kafka producer disposal");
        }
    }

    private static LogLevel ToLogLevel(SyslogLevel level) => level switch
    {
        SyslogLevel.Emergency => LogLevel.Critical,
        SyslogLevel.Alert => LogLevel.Critical,
        SyslogLevel.Critical => LogLevel.Critical,
        SyslogLevel.Error => LogLevel.Error,
        SyslogLevel.Warning => LogLevel.Warning,
        SyslogLevel.Notice => LogLevel.Information,
        SyslogLevel.Info => LogLevel.Information,
        SyslogLevel.Debug => LogLevel.Debug,
        _ => LogLevel.None,
    };
}