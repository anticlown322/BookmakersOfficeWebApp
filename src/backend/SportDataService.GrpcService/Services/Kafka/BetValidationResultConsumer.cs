using System.Text.Json;
using Confluent.Kafka;
using Grpc.Core;
using Microsoft.Extensions.Options;
using SportDataService.GrpcService.Contracts;
using SportDataService.GrpcService.Models.Kafka;
using SportDataService.GrpcService.Models.Settings;
using SportDataService.GrpcService.Utility;

namespace SportDataService.GrpcService.Services.Kafka;

public class BetValidationConsumer(
    IKafkaConsumerService consumerService,
    IKafkaProducerService producer,
    SportDataGrpcService sportDataService,
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<BetValidationConsumer> logger)
    : BackgroundService
{
    private readonly IConsumer<string, string> _consumer = consumerService.CreateConsumer();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _consumer.Subscribe(kafkaSettings.Value.Topics.BetValidation);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(ct);
                var message = JsonSerializer.Deserialize<BetValidationEvent>(consumeResult.Message.Value);
                
                if (message == null) continue;

                logger.LogInformation("Processing bet validation for {BetId}", message.BetId);

                var response = await ValidateBetAsync(message);
                await SendValidationResult(response, message.BetId);
                
                _consumer.Commit(consumeResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing bet validation");
            }
        }
    }

    private async Task<SportValidationResult> ValidateBetAsync(BetValidationEvent message)
    {
        try
        {
            var request = new ValidateBetRequest
            {
                MatchId = message.MatchId,
                LineType = message.LineType,
                MarketSelection = message.MarketSelection,
                Odds = message.RequestedOdds
            };

            var grpcResponse = await sportDataService.ValidateBet(request, null!);
            
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
            logger.LogError(ex, "Validation failed for bet {BetId}", message.BetId);
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

    private async Task SendValidationResult(SportValidationResult result, string betId)
    {
        await producer.ProduceAsync(
            kafkaSettings.Value.Topics.SportValidationResults,
            betId,
            result);
        
        logger.LogInformation("Sent sport validation result for bet {BetId}", betId);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}
