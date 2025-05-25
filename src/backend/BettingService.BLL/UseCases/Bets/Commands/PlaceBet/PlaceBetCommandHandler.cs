using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using Grpc.Core;
using Grpc.Net.Client;
using MediatR;
using Microsoft.Extensions.Logging;
using NLog;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed class PlaceBetCommandHandler(
    IBetRepository betRepository,
    UserGrpcService.UserGrpcServiceClient userClient,
    SportDataService.SportDataServiceClient sportDataClient,
    ILogger<PlaceBetCommandHandler> logger)
    : IRequestHandler<PlaceBetCommand, Unit>
{
    public async Task<Unit> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Checking balance for user {request.Username}");

        var balanceResponse = await userClient.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = request.Username },
            cancellationToken: cancellationToken);

        if (!balanceResponse.UserExists)
        {
            logger.LogWarning($"User {request.Username} does not exist");

            throw new UserNotFoundByNameException(request.Username);
        }

        if (balanceResponse.Balance < (double)request.PlaceBetDto.Amount)
        {
            logger.LogWarning($"User {request.Username} does not have enough amount");

            throw new InsufficientFundsException(request.PlaceBetDto.Amount);
        }

        logger.LogInformation($"Checking sport data for bet of user {request.Username}");

        var validationResponse = await sportDataClient.ValidateBetAsync(
            new ValidateBetRequest
            {
                MatchId = request.PlaceBetDto.MatchId,
                LineType = request.PlaceBetDto.LineType,
                MarketSelection = request.PlaceBetDto.MarketSelection,
                Odds = (double)request.PlaceBetDto.Odds,
            },
            cancellationToken: cancellationToken);

        if (!validationResponse.IsValid)
        {
            logger.LogWarning($"User {request.Username} bet validation failed");

            throw new InvalidBetParametersException();
        }

        logger.LogInformation($"Updating balance of user {request.Username}");

        var deductionResult = await userClient.UpdateUserBalanceAsync(
            new UpdateUserBalanceRequest
            {
                Username = request.Username,
                Amount = -(double)request.PlaceBetDto.Amount,
            },
            cancellationToken: cancellationToken);

        if (!deductionResult.Success)
        {
            logger.LogWarning($"User {request.Username} balance update failed");

            throw new BalanceUpdateFailedException(request.Username);
        }

        var bet = new Bet
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            MatchId = request.PlaceBetDto.MatchId,
            LineType = ParseLineType(request.PlaceBetDto.LineType),
            MarketSelection = request.PlaceBetDto.MarketSelection,
            Amount = request.PlaceBetDto.Amount,
            Odds = (decimal)validationResponse.CurrentOdds,
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

    private BetLineType ParseLineType(string lineType) => lineType switch
    {
        "MainLine" => BetLineType.Main,
        "KillsLine" => BetLineType.Kills,
        "MapsLine" => BetLineType.Maps,
        "SpecialLine" => BetLineType.Special,
        _ => throw new InvalidBetParametersException()
    };
}