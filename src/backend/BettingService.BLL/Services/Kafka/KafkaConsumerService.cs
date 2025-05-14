using System.Text.Json;
using System.Text.Json.Serialization;
using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Models.Settings.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Kafka;

public class KafkaConsumerService : IKafkaConsumerService, IDisposable
{
    private readonly IConsumer<string, string> _consumer;
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly KafkaSettings _kafkaSettings;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed = false;

    public KafkaConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaConsumerService> logger,
        JsonSerializerOptions? jsonOptions = null)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            UnknownTypeHandling = JsonUnknownTypeHandling.JsonNode,
            UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
            NumberHandling = JsonNumberHandling.AllowReadingFromString,
        };

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.ConsumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
            AllowAutoCreateTopics = false,
        };

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Reason}", e.Reason))
            .Build();
    }

    public async Task<T> ConsumeSingleMessageAsync<T>(
        string topic,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(this.ToString());
        }

        using var cts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);

        try
        {
            _consumer.Subscribe(topic);

            while (!linkedCts.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(linkedCts.Token);
                    if (consumeResult == null || consumeResult.IsPartitionEOF)
                    {
                        continue;
                    }

                    try
                    {
                        var message = JsonSerializer.Deserialize<T>(consumeResult.Message.Value, _jsonOptions);
                        if (message == null)
                        {
                            continue;
                        }

                        _consumer.Commit(consumeResult);
                        return message;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogError(ex, "Failed to deserialize message from {Topic}", topic);
                    }
                }
                catch (ConsumeException ex) when (ex.Error.IsFatal)
                {
                    _logger.LogError(ex, "Fatal error consuming from {Topic}", topic);
                    throw;
                }
                catch (ConsumeException ex)
                {
                    _logger.LogWarning(ex, "Non-fatal error consuming from {Topic}", topic);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Consumption from {Topic} was cancelled", topic);
                    throw new TimeoutException(
                        $"No message received from {topic} within {timeout.TotalSeconds} seconds");
                }
            }

            throw new TimeoutException($"No message received from {topic} within {timeout.TotalSeconds} seconds");
        }
        finally
        {
            _consumer.Unsubscribe();
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _consumer.Close();
            _consumer.Dispose();

            _disposed = true;
        }
    }
}