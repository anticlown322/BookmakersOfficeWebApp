using System.Text.Json;
using Confluent.Kafka;
using Grpc.Core;
using Microsoft.Extensions.Options;
using UserService.GrpcService.Contracts;
using UserService.GrpcService.Models.Kafka;
using UserService.GrpcService.Models.Settings;

namespace UserService.GrpcService.Services.Kafka;

public class PayoutRequestConsumer(
    IKafkaConsumerService consumerService,
    IKafkaProducerService producerService,
    UserGrpcServiceImplementation userService,
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<PayoutRequestConsumer> logger)
    : BackgroundService
{
    private readonly IConsumer<string, string> _consumer = consumerService.CreateConsumer();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _consumer.Subscribe(kafkaSettings.Value.Topics.PayoutRequests);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(ct);
                var payoutRequest = JsonSerializer.Deserialize<PayoutRequest>(consumeResult.Message.Value);

                if (payoutRequest == null)
                {
                    logger.LogWarning("Received null payout request");
                    continue;
                }

                logger.LogInformation(
                    "Processing payout {PayoutId} for user {Username} and bet {BetId}",
                    payoutRequest.PayoutId,
                    payoutRequest.Username,
                    payoutRequest.BetId);

                var response = await ProcessPayoutAsync(payoutRequest, ct);
                await SendPayoutResult(response, payoutRequest.PayoutId);

                _consumer.Commit(consumeResult);
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Payout processing was cancelled");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing payout request");
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
            var updateResult = await userService.UpdateUserBalance(
                new UpdateUserBalanceRequest
                {
                    Username = request.Username,
                    Amount = (double)request.Amount,
                },
                null!);

            if (updateResult.Success)
            {
                result.IsSuccess = true;
                result.NewBalance = (decimal)updateResult.NewBalance;
            }
            else
            {
                result.IsSuccess = false;
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            result.IsSuccess = false;
            result.ErrorMessage = $"User {request.Username} not found";
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = $"Internal error: {ex.Message}";
        }

        return result;
    }

    private async Task SendPayoutResult(PayoutResult result, string payoutId)
    {
        await producerService.ProduceAsync(
            kafkaSettings.Value.Topics.PayoutResults,
            result);

        logger.LogInformation(
            "Sent payout result for {PayoutId}. Success: {IsSuccess}",
            payoutId,
            result.IsSuccess);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}