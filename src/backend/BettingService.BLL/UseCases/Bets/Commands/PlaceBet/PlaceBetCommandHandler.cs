using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.Kafka;
using BettingService.DAL.Models.Settings.Kafka;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed class PlaceBetCommandHandler(
    IBetRepository betRepository,
    IKafkaProducerService kafkaProducer,
    IKafkaConsumerService kafkaConsumer,
    IOptions<KafkaSettings> kafkaSettings)
    : IRequestHandler<PlaceBetCommand, Unit>
{
    public async Task<Unit> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString();
        var betId = Guid.NewGuid().ToString();

        var userValidationRequest = new BetValidationRequest
        {
            BetId = betId,
            CorrelationId = correlationId,
            ValidationType = "User",
            Username = request.Username,
            Amount = (double)request.PlaceBetDto.Amount,
            Timestamp = DateTime.UtcNow,
        };

        await kafkaProducer.ProduceAsync(
            kafkaSettings.Value.Topics.BetValidationRequests,
            userValidationRequest,
            cancellationToken);

        var userValidationResult = await WaitForValidationResult<UserValidationResult>(
            kafkaSettings.Value.Topics.UserValidationResults,
            correlationId,
            TimeSpan.FromSeconds(30),
            cancellationToken);

        if (!userValidationResult.IsValid)
        {
            throw new UserValidationFailedException(userValidationResult.RejectionReason);
        }

        var sportValidationRequest = new BetValidationRequest
        {
            BetId = betId,
            CorrelationId = correlationId,
            ValidationType = "Sport",
            MatchId = request.PlaceBetDto.MatchId,
            LineType = request.PlaceBetDto.LineType,
            MarketSelection = request.PlaceBetDto.MarketSelection,
            RequestedOdds = (double)request.PlaceBetDto.Odds,
            Timestamp = DateTime.UtcNow,
        };

        await kafkaProducer.ProduceAsync(
            kafkaSettings.Value.Topics.BetValidationRequests,
            sportValidationRequest,
            cancellationToken);

        var sportValidationResult = await WaitForValidationResult<SportValidationResult>(
            kafkaSettings.Value.Topics.SportValidationResults,
            correlationId,
            TimeSpan.FromSeconds(30),
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
            Odds = (decimal)sportValidationResult.CurrentOdds,
            Status = BetStatus.Pending,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            AcceptedAt = DateTime.UtcNow.ToUniversalTime(),
        };

        betRepository.Create(bet);
        await betRepository.SaveAsync(cancellationToken);

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
                TimeSpan.FromSeconds(5),
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