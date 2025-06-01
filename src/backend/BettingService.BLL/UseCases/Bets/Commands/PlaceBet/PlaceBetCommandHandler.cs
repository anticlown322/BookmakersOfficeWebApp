using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.MessageBroker;
using BettingService.DAL.Models.Settings.Kafka;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Logging;
using NLog;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed class PlaceBetCommandHandler(
    IBetRepository betRepository,
    UserGrpcService.UserGrpcServiceClient userClient,
    SportDataService.SportDataServiceClient sportDataClient,
    ILogger<PlaceBetCommandHandler> logger,
    IKafkaProducerService kafkaProducer,
    IKafkaConsumerService kafkaConsumer,
    IOptions<KafkaSettings> kafkaSettings)
    : IRequestHandler<PlaceBetCommand, Unit>
{
    private readonly KafkaSettings _kafkaSettings = kafkaSettings.Value;

    public async Task<Unit> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Checking balance for user {request.Username}");

        var correlationId = Guid.NewGuid().ToString();
        var betId = Guid.NewGuid().ToString();

        var userValidationRequest = new UserValidationRequest
        {
            BetId = betId,
            CorrelationId = correlationId,
            Username = request.Username,
            Amount = request.PlaceBetDto.Amount,
            Timestamp = DateTime.UtcNow,
        };

        logger.LogInformation($"Validating user {request.Username}");

        await kafkaProducer.ProduceAsync(
            _kafkaSettings.Topics.UserValidationRequests,
            userValidationRequest,
            cancellationToken);

        var userValidationResult = await WaitForValidationResult<UserValidationResult>(
            _kafkaSettings.Topics.UserValidationResults,
            correlationId,
            TimeSpan.FromSeconds(_kafkaSettings.RequestTimeoutSeconds),
            cancellationToken);

        if (!userValidationResult.IsValid)
        {
            throw new UserValidationFailedException(userValidationResult.RejectionReason);
        }

        logger.LogInformation($"Checking sport data for bet of user {request.Username}");

        var sportValidationRequest = new SportValidationRequest
        {
            BetId = betId,
            CorrelationId = correlationId,
            MatchId = request.PlaceBetDto.MatchId,
            LineType = request.PlaceBetDto.LineType,
            MarketSelection = request.PlaceBetDto.MarketSelection,
            RequestedOdds = request.PlaceBetDto.Odds,
            Timestamp = DateTime.UtcNow,
        };

        await kafkaProducer.ProduceAsync(
            _kafkaSettings.Topics.SportValidationRequests,
            sportValidationRequest,
            cancellationToken);

        var sportValidationResult = await WaitForValidationResult<SportValidationResult>(
            _kafkaSettings.Topics.SportValidationResults,
            correlationId,
            TimeSpan.FromSeconds(_kafkaSettings.RequestTimeoutSeconds),
            cancellationToken);

        if (!sportValidationResult.IsValid)
        {
            throw new SportValidationFailedException(sportValidationResult.RejectionReason);
        }

        var bet = new Bet
        {
            Id = Guid.Parse(betId),
            Username = request.Username,
            MatchId = request.PlaceBetDto.MatchId,
            LineType = ParseLineType(request.PlaceBetDto.LineType),
            MarketSelection = request.PlaceBetDto.MarketSelection,
            Amount = request.PlaceBetDto.Amount,
            Odds = sportValidationResult.CurrentOdds,
            Status = BetStatus.Pending,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            AcceptedAt = DateTime.UtcNow.ToUniversalTime(),
        };

        logger.LogInformation($"Saving bet...");

        betRepository.Create(bet);
        await betRepository.SaveAsync(cancellationToken);

        logger.LogInformation($"Successfully placed bet");

        return Unit.Value;
    }

    private async Task<T> WaitForValidationResult<T>(
        string topic,
        string correlationId,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            var message = await kafkaConsumer.ConsumeSingleMessageAsync<T>(
                topic,
                TimeSpan.FromSeconds(_kafkaSettings.ConsumeMsgTimeoutSeconds),
                cancellationToken);

            if (message != null && GetCorrelationId(message) == correlationId)
            {
                return message;
            }
        }

        throw new TimeoutException($"Validation result not received within {timeout.TotalSeconds} seconds");
    }

    private string GetCorrelationId(object message)
    {
        return message switch
        {
            UserValidationResult uvr => uvr.CorrelationId,
            SportValidationResult svr => svr.CorrelationId,
            _ => throw new InvalidOperationException("Unknown message type")
        };
    }

    private BetLineType ParseLineType(string lineType) => lineType switch
    {
        "MainLine" => BetLineType.Main,
        "KillsLine" => BetLineType.Kills,
        "MapsLine" => BetLineType.Maps,
        "SpecialLine" => BetLineType.Special,
        _ => throw new InvalidBetParametersException()
    };
}