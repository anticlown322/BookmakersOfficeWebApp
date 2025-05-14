using Grpc.Core;
using Microsoft.Extensions.Options;
using UserService.Domain.Models;
using UserService.Infrastructure.Services.EventBus.Kafka.Abstractions;
using UserService.Infrastructure.Services.EventBus.Kafka.Implementations;
using UserService.Infrastructure.Services.EventBus.Kafka.Settings;

namespace UserService.GrpcService.Services;

public class PayoutRequestConsumer : BackgroundService
{
    private readonly IEventConsumer<PayoutRequest> _consumer;
    private readonly IEventProducer _producer;
    private readonly UserGrpcServiceImplementation _userService;
    private readonly ILogger<PayoutRequestConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public PayoutRequestConsumer(
        IEventConsumer<PayoutRequest> consumer,
        IEventProducer producer,
        UserGrpcServiceImplementation userService,
        ILogger<PayoutRequestConsumer> logger,
        IOptions<KafkaSettings> kafkaSettings)
    {
        _consumer = consumer;
        _producer = producer;
        _userService = userService;
        _logger = logger;
        _kafkaSettings = kafkaSettings.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        await _consumer.SubscribeAsync(_kafkaSettings.Topics.PayoutRequests, ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await _consumer.ConsumeAsync(ct);
                var payoutRequest = result.Message;

                _logger.LogInformation(
                    "Processing payout {PayoutId} for user {Username}",
                    payoutRequest.PayoutId,
                    payoutRequest.Username);

                var payoutResult = await ProcessPayoutAsync(payoutRequest, ct);
                await _producer.ProduceAsync(
                    _kafkaSettings.Topics.PayoutResults,
                    payoutResult.PayoutId,
                    payoutResult,
                    ct);

                await _consumer.CommitAsync(result, ct);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Payout processing cancelled");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payout processing error");
            }
        }
    }

    private async Task<PayoutResult> ProcessPayoutAsync(PayoutRequest request, CancellationToken ct)
    {
        var result = new PayoutResult
        {
            PayoutId = request.PayoutId,
            CorrelationId = request.CorrelationId,
            Timestamp = DateTime.UtcNow,
        };

        try
        {
            var updateResult = await _userService.UpdateUserBalance(
                new UpdateUserBalanceRequest
                {
                    Username = request.Username,
                    Amount = (double)request.Amount,
                },
                null!);

            result.IsSuccess = updateResult.Success;
            result.NewBalance = (decimal)updateResult.NewBalance;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            result.ErrorMessage = $"User {request.Username} not found";
        }
        catch (Exception ex)
        {
            result.ErrorMessage = $"Internal error: {ex.Message}";
        }

        return result;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        await base.StopAsync(ct);
        _consumer.Dispose();
    }
}