using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed class PlaceBetCommandHandler(
    IBetRepository betRepository,
    UserGrpcService.UserGrpcServiceClient userClient,
    SportDataService.SportDataServiceClient sportDataClient)
    : IRequestHandler<PlaceBetCommand, Unit>
{
    public async Task<Unit> Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        var balanceResponse = await userClient.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = request.Username },
            cancellationToken: cancellationToken);

        if (!balanceResponse.UserExists)
        {
            throw new UserNotFoundByNameException(request.Username);
        }

        if (balanceResponse.Balance < (double)request.PlaceBetDto.Amount)
        {
            throw new InsufficientFundsException(request.PlaceBetDto.Amount);
        }

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
            throw new InvalidBetParametersException();
        }

        var deductionResult = await userClient.UpdateUserBalanceAsync(
            new UpdateUserBalanceRequest
            {
                Username = request.Username,
                Amount = -(double)request.PlaceBetDto.Amount,
            },
            cancellationToken: cancellationToken);

        if (!deductionResult.Success)
        {
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

        betRepository.Create(bet);
        await betRepository.SaveAsync(cancellationToken);

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