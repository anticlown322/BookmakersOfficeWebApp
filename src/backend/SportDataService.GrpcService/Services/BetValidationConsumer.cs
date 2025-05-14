using Grpc.Core;
using Microsoft.Extensions.Options;
using SportDataService.Domain.Models.Settings;
using SportDataService.Infrastructure.Services.EventBus.Kafka.Abstractions;
using SportDataService.Infrastructure.Services.EventBus.Kafka.Implementations;

namespace SportDataService.GrpcService.Services;

public class BetValidationConsumer : BackgroundService, IDisposable
{
    private readonly IEventConsumer<SportValidationEvent> _consumer;
    private readonly IEventProducer _producer;
    private readonly SportDataGrpcService _sportDataService;
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<BetValidationConsumer> _logger;
    private bool _disposed;

    public BetValidationConsumer(
        IEventConsumer<SportValidationEvent> consumer,
        IEventProducer producer,
        SportDataGrpcService sportDataService,
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<BetValidationConsumer> logger)
    {
        _consumer = consumer;
        _producer = producer;
        _sportDataService = sportDataService;
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
        _disposed = false;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(BetValidationConsumer));
        }

        await _consumer.SubscribeAsync(_kafkaSettings.Topics.SportValidationRequests, ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await _consumer.ConsumeAsync(ct);
                var validationResult = await ValidateBetAsync(result.Message);

                await _producer.ProduceAsync(
                    _kafkaSettings.Topics.SportValidationResults,
                    validationResult.BetId,
                    validationResult, 
                    ct);

                await _consumer.CommitAsync(result, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
            }
        }
    }

    private async Task<SportValidationResult> ValidateBetAsync(SportValidationEvent message)
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(BetValidationConsumer));
        }

        try
        {
            var request = new ValidateBetRequest
            {
                MatchId = message.MatchId,
                LineType = message.LineType,
                MarketSelection = message.MarketSelection,
                Odds = message.RequestedOdds
            };

            var grpcResponse = await _sportDataService.ValidateBet(request, null!);

            return new SportValidationResult
            {
                BetId = message.BetId,
                CorrelationId = message.CorrelationId,
                IsValid = grpcResponse.IsValid,
                CurrentOdds = grpcResponse.CurrentOdds,
                Timestamp = DateTime.UtcNow.ToUniversalTime(),
            };
        }
        catch (RpcException ex)
        {
            _logger.LogError(ex, "Validation failed for bet {BetId}", message.BetId);
            return new SportValidationResult
            {
                BetId = message.BetId,
                CorrelationId = message.CorrelationId,
                IsValid = false,
                RejectionReason = ex.Message,
                Timestamp = DateTime.UtcNow.ToUniversalTime()
            };
        }
    }

    public override void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;

            _consumer.Dispose();
            base.Dispose();
        }
    }
}