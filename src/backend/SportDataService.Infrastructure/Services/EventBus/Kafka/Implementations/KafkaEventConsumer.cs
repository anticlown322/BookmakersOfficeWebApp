using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportDataService.Domain.Models.Settings;
using SportDataService.Infrastructure.Services.EventBus.Kafka.Abstractions;

namespace SportDataService.Infrastructure.Services.EventBus.Kafka.Implementations;

public class KafkaEventConsumer<T> : IEventConsumer<T>
    where T : class
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaEventConsumer<T>> _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly KafkaSettings _settings;
    private bool _disposed;

    public KafkaEventConsumer(
        IOptions<KafkaSettings> settings,
        ILogger<KafkaEventConsumer<T>> logger,
        JsonSerializerOptions? jsonOptions = null)
    {
        _logger = logger;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        };
        _settings = settings.Value;

        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = $"{_settings.ConsumerGroup}.sportdata",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            IsolationLevel = IsolationLevel.ReadCommitted,
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler(LogError)
            .Build();

        _disposed = false;
    }

    public async Task SubscribeAsync(string topic, CancellationToken ct = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaEventProducer));
        }

        using var adminClient = new AdminClientBuilder(
            new AdminClientConfig
            {
                BootstrapServers = _settings.BootstrapServers,
            }).Build();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(10), ct);

            _consumer.Subscribe(topic);
            _logger.LogInformation("Successfully subscribed to {Topic}", topic);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to subscribe to topic {Topic}", topic);
            throw;
        }
    }

    public async Task<ConsumeResult<T>> ConsumeAsync(CancellationToken ct)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaEventProducer));
        }

        try
        {
            var result = _consumer.Consume(ct);
            var message = JsonSerializer.Deserialize<T>(result.Message.Value, _jsonOptions);

            if (message == null)
            {
                throw new InvalidOperationException("Failed to deserialize message");
            }

            return new ConsumeResult<T>(
                message,
                result.Topic,
                result.Partition,
                result.Offset.Value);
        }
        catch (ConsumeException ex)
        {
            _logger.LogError(ex, "Consume error");
            throw;
        }
    }

    public async Task CommitAsync(ConsumeResult<T> result, CancellationToken ct = default)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(KafkaEventProducer));
        }

        var offset = new TopicPartitionOffset(
            result.Topic,
            new Partition(result.Partition),
            new Offset(result.Offset + 1));

        _consumer.Commit(new[] { offset });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            _consumer.Close();
            _consumer.Dispose();
        }
    }

    private void LogError(IConsumer<string, string> _, Error error)
        => _logger.LogError("Kafka error: {Reason}", error.Reason);
}