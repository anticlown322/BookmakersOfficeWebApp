using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed class PlaceBetCommandHandler(
    IBetRepository betRepository)
    : IRequestHandler<PlaceBetCommand>
{
    public async Task Handle(PlaceBetCommand request, CancellationToken cancellationToken)
    {
        var bet = new Bet
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            MatchId = request.PlaceBetDto.MatchId,
            Amount = request.PlaceBetDto.Amount,
            Odds = request.PlaceBetDto.Odds,
            Status = BetStatus.Active,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
        };

        betRepository.Create(bet);

        cancellationToken.ThrowIfCancellationRequested();

        await betRepository.SaveAsync(cancellationToken);
    }
}