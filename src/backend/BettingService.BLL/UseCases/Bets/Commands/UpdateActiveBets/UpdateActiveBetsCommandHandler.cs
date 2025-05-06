using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.UpdateActiveBets;

public class UpdateActiveBetsCommandHandler(
    IBetRepository betRepository,
    SportDataService.SportDataServiceClient sportDataClient)
    : IRequestHandler<UpdateActiveBetsCommand, Unit>
{
    public async Task<Unit> Handle(UpdateActiveBetsCommand request, CancellationToken cancellationToken)
    {
        var activeBets = await betRepository.FindByConditionAsync(
            b => b.Status == BetStatus.Active,
            trackChanges: true,
            cancellationToken);

        if (!activeBets.Any())
        {
            return Unit.Value;
        }

        var matchIds = activeBets
            .Select(b => b.MatchId)
            .Distinct()
            .ToList();

        var resultsResponse = await sportDataClient.GetMatchResultsBatchAsync(
            new GetMatchResultsBatchRequest { MatchIds = { matchIds } },
            cancellationToken: cancellationToken);

        var resultsDict = resultsResponse.Results.ToDictionary(r => r.MatchId);

        foreach (var bet in activeBets)
        {
            if (resultsDict.TryGetValue(bet.MatchId, out var result))
            {
                ProcessBetResult(bet, result);
                betRepository.Update(bet);
            }
        }

        await betRepository.SaveAsync(cancellationToken);

        return Unit.Value;
    }

    private void ProcessBetResult(Bet bet, MatchResultData result)
    {
        bet.SettledAt = DateTime.UtcNow;

        if (result.Status == ResultStatus.Canceled)
        {
            bet.Status = BetStatus.Refunded;
            bet.CancelledAt = DateTime.UtcNow.ToUniversalTime();
            return;
        }

        if (result.Status != ResultStatus.Ended)
        {
            return;
        }

        var marketKey = $"{bet.LineType}_{bet.MarketSelection}";

        if (result.Outcomes.TryGetValue(marketKey, out var outcome))
        {
            bet.Status = outcome switch
            {
                "won" => BetStatus.Won,
                "lost" => BetStatus.Lost,
                _ => BetStatus.Refunded
            };
        }
        else
        {
            bet.Status = BetStatus.Refunded;
        }
    }
}