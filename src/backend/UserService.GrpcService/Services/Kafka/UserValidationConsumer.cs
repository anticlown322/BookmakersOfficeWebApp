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

                var response = await ValidateUserAsync(message, ct);
                await SendValidationResult(response, message.BetId);

                _consumer.Commit(consumeResult);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error processing user validation");
            }
        }
    }

    private async Task<UserValidationResult> ValidateUserAsync(BetValidationEvent message, CancellationToken ct)
    {
        try
        {
            var balanceRequest = new GetUserBalanceRequest { Username = message.Username };

            var balanceResponse = await userService.GetUserBalance(balanceRequest, null!);

            if (!balanceResponse.UserExists)
            {
                return new UserValidationResult
                {
                    BetId = message.BetId,
                    CorrelationId = message.CorrelationId,
                    IsValid = false,
                    RejectionReason = "User not found",
                    Timestamp = DateTime.UtcNow.ToUniversalTime(),
                };
            }

            if (balanceResponse.Balance < message.Amount)
            {
                return new UserValidationResult
                {
                    BetId = message.BetId,
                    CorrelationId = message.CorrelationId,
                    IsValid = false,
                    RejectionReason = "Insufficient funds",
                    CurrentBalance = balanceResponse.Balance,
                    Timestamp = DateTime.UtcNow.ToUniversalTime(),
                };
            }

            return new UserValidationResult
            {
                BetId = message.BetId,
                CorrelationId = message.CorrelationId,
                IsValid = true,
                CurrentBalance = balanceResponse.Balance,
                Timestamp = DateTime.UtcNow.ToUniversalTime(),
            };
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return new UserValidationResult
            {
                BetId = message.BetId,
                CorrelationId = message.CorrelationId,
                IsValid = false,
                RejectionReason = "User not found",
                CurrentBalance = 0,
                Timestamp = DateTime.UtcNow.ToUniversalTime(),
            };
        }
        catch (RpcException)
        {
            return new UserValidationResult
            {
                BetId = message.BetId,
                CorrelationId = message.CorrelationId,
                IsValid = false,
                RejectionReason = "Error while validating user",
                CurrentBalance = 0,
                Timestamp = DateTime.UtcNow.ToUniversalTime(),
            };
        }
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