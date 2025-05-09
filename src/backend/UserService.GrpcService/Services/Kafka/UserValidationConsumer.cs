using System.Text.Json;
using Confluent.Kafka;
using Grpc.Core;
using Microsoft.Extensions.Options;
using UserService.GrpcService.Contracts;
using UserService.GrpcService.Models.Kafka;
using UserService.GrpcService.Models.Settings;

namespace UserService.GrpcService.Services.Kafka;

public class UserValidationConsumer(
    IKafkaConsumerService consumerService,
    IKafkaProducerService producer,
    UserGrpcServiceImplementation userService,
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<UserValidationConsumer> logger)
    : BackgroundService
{
    private readonly IConsumer<string, string> _consumer = consumerService.CreateConsumer();

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _consumer.Subscribe(kafkaSettings.Value.Topics.BetValidationRequests);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var consumeResult = _consumer.Consume(ct);
                var message = JsonSerializer.Deserialize<BetValidationEvent>(consumeResult.Message.Value);

                if (message == null)
                {
                    continue;
                }

                logger.LogInformation("Validating user for bet {BetId}", message.BetId);

                var response = await ProcessUserBetAsync(message, ct);
                await SendValidationResult(response, message.BetId);

                _consumer.Commit(consumeResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing user validation");
            }
        }
    }

    private async Task<UserValidationResult> ProcessUserBetAsync(BetValidationEvent message, CancellationToken ct)
    {
        try
        {
            var balanceRequest = new GetUserBalanceRequest { Username = message.Username };
            var balanceResponse = await userService.GetUserBalance(balanceRequest, null!);

            if (!balanceResponse.UserExists)
            {
                return CreateFailedValidationResult(message, "User not found");
            }

            if (balanceResponse.Balance < message.Amount)
            {
                return CreateFailedValidationResult(message, "Insufficient funds", balanceResponse.Balance);
            }

            var deductionRequest = new UpdateUserBalanceRequest
            {
                Username = message.Username,
                Amount = -message.Amount,
            };

            var deductionResponse = await userService.UpdateUserBalance(deductionRequest, null!);

            if (!deductionResponse.Success)
            {
                return CreateFailedValidationResult(
                    message,
                    $"Balance update failed.",
                    balanceResponse.Balance);
            }

            // 3. Возвращаем успешный результат
            return new UserValidationResult
            {
                BetId = message.BetId,
                CorrelationId = message.CorrelationId,
                IsValid = true,
                CurrentBalance = deductionResponse.NewBalance,
                Timestamp = DateTime.UtcNow,
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return CreateFailedValidationResult(message, "User not found");
        }
        catch (RpcException ex)
        {
            return CreateFailedValidationResult(message, $"gRPC error: {ex.Status.Detail}");
        }
        catch (Exception ex)
        {
            return CreateFailedValidationResult(message, $"Internal error: {ex.Message}");
        }
    }

    private UserValidationResult CreateFailedValidationResult(
        BetValidationEvent message,
        string reason,
        double? currentBalance = null)
    {
        return new UserValidationResult
        {
            BetId = message.BetId,
            CorrelationId = message.CorrelationId,
            IsValid = false,
            RejectionReason = reason,
            CurrentBalance = currentBalance ?? 0,
            Timestamp = DateTime.UtcNow,
        };
    }

    private async Task SendValidationResult(UserValidationResult result, string betId)
    {
        await producer.ProduceAsync(
            kafkaSettings.Value.Topics.UserValidationResults,
            betId,
            result);

        logger.LogInformation("Sent user validation result for bet {BetId}", betId);
    }

    public override void Dispose()
    {
        _consumer.Close();
        _consumer.Dispose();
        base.Dispose();
    }
}