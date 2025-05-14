using Grpc.Core;
using Microsoft.Extensions.Options;
using UserService.Domain.Models;
using UserService.Infrastructure.Services.EventBus.Kafka.Abstractions;
using UserService.Infrastructure.Services.EventBus.Kafka.Implementations;
using UserService.Infrastructure.Services.EventBus.Kafka.Settings;

namespace UserService.GrpcService.Services;

public class UserValidationConsumer : BackgroundService
{
    private readonly IEventConsumer<UserValidationEvent> _consumer;
    private readonly IEventProducer _producer;
    private readonly UserGrpcServiceImplementation _userService;
    private readonly ILogger<UserValidationConsumer> _logger;
    private readonly KafkaSettings _kafkaSettings;

    public UserValidationConsumer(
        IEventConsumer<UserValidationEvent> consumer,
        IEventProducer producer,
        UserGrpcServiceImplementation userService,
        ILogger<UserValidationConsumer> logger,
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
        await _consumer.SubscribeAsync(_kafkaSettings.Topics.UserValidationRequests, ct);

        while (!ct.IsCancellationRequested)
        {
            try
            {
                var result = await _consumer.ConsumeAsync(ct);
                var message = result.Message;

                _logger.LogInformation("Validating user for bet {BetId}", message.BetId);

                var validationResult = await ValidateUserBetAsync(message, ct);
                await _producer.ProduceAsync(
                    _kafkaSettings.Topics.UserValidationResults,
                    message.BetId,
                    validationResult,
                    ct);

                await _consumer.CommitAsync(result, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation error");
            }
        }
    }

    private async Task<UserValidationResult> ValidateUserBetAsync(
        UserValidationEvent message,
        CancellationToken ct)
    {
        try
        {
            double grpcMessageAmount = (double)message.Amount;
            var balanceResponse = await _userService.GetUserBalance(
                new GetUserBalanceRequest { Username = message.Username },
                null!);

            if (!balanceResponse.UserExists)
            {
                return CreateFailedResult(message, "User not found");
            }

            if (balanceResponse.Balance < grpcMessageAmount)
            {
                return CreateFailedResult(message, "Insufficient funds", balanceResponse.Balance);
            }

            var deductionResponse = await _userService.UpdateUserBalance(
                new UpdateUserBalanceRequest
                {
                    Username = message.Username,
                    Amount = -grpcMessageAmount,
                },
                null!);

            return deductionResponse.Success
                ? CreateSuccessResult(message, deductionResponse.NewBalance)
                : CreateFailedResult(message, "Balance update failed", balanceResponse.Balance);
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return CreateFailedResult(message, "User not found");
        }
        catch (Exception ex)
        {
            return CreateFailedResult(message, $"Error: {ex.Message}");
        }
    }

    private static UserValidationResult CreateSuccessResult(
        UserValidationEvent message,
        double newBalance)
    {
        var result = new UserValidationResult()
        {
            BetId = message.BetId,
            CorrelationId = message.CorrelationId,
            IsValid = true,
            CurrentBalance = newBalance,
            Timestamp = DateTime.UtcNow,
        };

        return result;
    }

    private static UserValidationResult CreateFailedResult(
        UserValidationEvent message,
        string reason,
        double? balance = null)
    {
        var result = new UserValidationResult()
        {
            BetId = message.BetId,
            CorrelationId = message.CorrelationId,
            IsValid = false,
            RejectionReason = reason,
            CurrentBalance = balance ?? 0,
            Timestamp = DateTime.UtcNow,
        };

        return result;
    }

    public override async Task StopAsync(CancellationToken ct)
    {
        await base.StopAsync(ct);
        _consumer.Dispose();
    }
}