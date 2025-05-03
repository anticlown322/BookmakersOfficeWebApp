using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.UpdatePendingBets;

public class UpdatePendingBetsCommandHandler(IBetRepository betRepository)
    : IRequestHandler<UpdatePendingBetsCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePendingBetsCommand request, CancellationToken cancellationToken)
    {
        var pendingBets = await betRepository.FindByConditionAsync(
            b => b.Status == BetStatus.Pending,
            trackChanges: true,
            cancellationToken);

        if (!pendingBets.Any())
        {
            return Unit.Value;
        }

        var now = DateTime.UtcNow;

        foreach (var bet in pendingBets)
        {
            bet.Status = BetStatus.Active;
            bet.AcceptedAt = now;
            betRepository.Update(bet);
        }

        await betRepository.SaveAsync(cancellationToken);

        return Unit.Value;
    }
}