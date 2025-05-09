using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.Kafka.BetValidation;
using BettingService.DAL.Models.Settings.Kafka;
using BettingService.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NLog;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed class PlaceBetCommandHandler(
    IBetRepository betRepository,
    IKafkaProducerService kafkaProducer,
    IOptions<KafkaSettings> kafkaSettings,
    ILogger<PlaceBetCommandHandler> logger)
    : IRequestHandler<PlaceBetCommand, BetPlacementResult>
{
    public async Task<BetPlacementResult> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        var bet = new Bet
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            MatchId = request.PlaceBetDto.MatchId,
            LineType = ParseLineType(request.PlaceBetDto.LineType),
            MarketSelection = request.PlaceBetDto.MarketSelection,
            Amount = request.PlaceBetDto.Amount,
            Odds = request.PlaceBetDto.Odds,
            Status = BetStatus.Validating,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
            AcceptedAt = null,
        };

        betRepository.Create(bet);
        logger.LogInformation("Created bet {BetId} in Validating state", bet.Id);

        var validationEvent = new BetValidationEvent
        {
            BetId = bet.Id.ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            Username = request.Username,
            Amount = (double)request.PlaceBetDto.Amount,
            MatchId = request.PlaceBetDto.MatchId,
            LineType = request.PlaceBetDto.LineType,
            MarketSelection = request.PlaceBetDto.MarketSelection,
            RequestedOdds = (double)request.PlaceBetDto.Odds,
            Timestamp = DateTime.UtcNow.ToUniversalTime(),
        };

        try
        {
            await kafkaProducer.ProduceAsync(
                topic: kafkaSettings.Value.Topics.BetValidation,
                key: bet.Id.ToString(),
                message: validationEvent,
                ct: cancellationToken);

            logger.LogInformation("Sent validation event for bet {BetId}", bet.Id);
            return new BetPlacementResult(bet.Id, BetPlacementStatus.ValidationPending);
        }
        catch (Exception ex)
        {
            bet.Status = BetStatus.Rejected;
            bet.RejectionReason = "Failed to start validation";
            betRepository.Update(bet);

            logger.LogError(ex, "Failed to validate bet {BetId}", bet.Id);
            return new BetPlacementResult(bet.Id, BetPlacementStatus.Rejected);
        }
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