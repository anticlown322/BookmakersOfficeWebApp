using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UserService.Domain.Models;
using UserService.Infrastructure.Services.EventBus.Kafka.Abstractions;
using UserService.Infrastructure.Services.EventBus.Kafka.Settings;

namespace UserService.Infrastructure.Services.EventBus.Kafka.Implementations;

public class KafkaEventProducer : IEventProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaEventProducer> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly KafkaSettings _settings;
    private bool _disposed;

    public KafkaEventProducer(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaEventProducer> logger,
        JsonSerializerOptions? jsonOptions = null)
    {
        _logger = logger;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions();
        _settings = settings.Value;

        var config = new ProducerConfig
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

        _producer = new ProducerBuilder<string, string>(config)
            .SetLogHandler(LogHandler)
            .Build();

        _disposed = false;
    }

    public async Task ProduceAsync<T>(string topic, T message, CancellationToken ct = default)
        where T : class
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaEventProducer));
        }

        await ProduceAsync(topic, null!, message, ct);
    }

    public async Task ProduceAsync<T>(string topic, string key, T message, CancellationToken ct = default)
        where T : class
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaEventProducer));
        }

        try
        {
            var json = JsonSerializer.Serialize(message, _jsonOptions);
            var kafkaMessage = new Message<string, string> { Key = key, Value = json };

            var result = await _producer.ProduceAsync(topic, kafkaMessage, ct);
            LogDelivery(result);
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex, "Failed to deliver message to {Topic}", topic);
            throw;
        }
    }

    public async Task FlushAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaEventProducer));
        }

        try
        {
            _producer.Flush(timeout);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during flush");
            throw;
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            _producer.Flush(TimeSpan.FromSeconds(5));
            _producer.Dispose();
        }
    }

    private void LogHandler(IProducer<string, string> _, LogMessage logMessage)
        => _logger.Log(ToLogLevel(logMessage.Level), logMessage.Message);

    private void LogDelivery(DeliveryResult<string, string> result)
        => _logger.LogDebug(
            "Delivered to {Topic} [P:{Partition}, O:{Offset}]",
            result.Topic,
            result.Partition,
            result.Offset);

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
        _ => LogLevel.None
    };
}