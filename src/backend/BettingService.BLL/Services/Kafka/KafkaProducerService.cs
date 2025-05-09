using System.Text.Json;
using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Models.Settings.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Kafka;

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

        var config = new ProducerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageTimeoutMs = 15000,
            RequestTimeoutMs = 10000,
            SocketTimeoutMs = 10000,
            Debug = "broker,protocol,msg", // Включите детальное логирование
            SecurityProtocol = SecurityProtocol.Plaintext, // Явно укажите протокол
            SslEndpointIdentificationAlgorithm = SslEndpointIdentificationAlgorithm.None
        };

        _producer = new ProducerBuilder<string, string>(config)
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
            _logger.LogInformation("Attempting to produce message to {Topic}. Key: {Key}, Value: {Value}", 
                topic, key, json); // Логируем перед отправкой

            var kafkaMessage = new Message<string, string> { Key = key, Value = json };
            var deliveryResult = await _producer.ProduceAsync(topic, kafkaMessage, ct);

            _logger.LogInformation(
                "Successfully delivered message to {Topic} [Partition: {Partition}, Offset: {Offset}, Status: {Status}]",
                deliveryResult.Topic,
                deliveryResult.Partition,
                deliveryResult.Offset,
                deliveryResult.Status);
        }
        catch (Exception ex) // Ловим все исключения, а не только ProduceException
        {
            _logger.LogError(ex, "Failed to produce message to {Topic}", topic);
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
        _ => LogLevel.None
    };
}