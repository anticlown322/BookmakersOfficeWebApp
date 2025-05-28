using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Bets.Commands.UpdatePendingBets;

public class UpdatePendingBetsCommandHandler(
    IBetRepository betRepository,
    ILogger<UpdatePendingBetsCommandHandler> logger)
    : IRequestHandler<UpdatePendingBetsCommand, Unit>
{
    public async Task<Unit> Handle(UpdatePendingBetsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting pending bets...");

        var pendingBets = await betRepository.FindByConditionAsync(
            b => b.Status == BetStatus.Pending,
            trackChanges: true,
            cancellationToken);

        if (!pendingBets.Any())
        {
            logger.LogInformation("No pending bets found");

            return Unit.Value;
        }

        var now = DateTime.UtcNow;

        logger.LogInformation("Changing bets status...");

        foreach (var bet in pendingBets)
        {
            bet.Status = BetStatus.Active;
            bet.AcceptedAt = now;
            betRepository.Update(bet);
        }

        await betRepository.SaveAsync(cancellationToken);

        logger.LogInformation($"Successfully placed pending bets");

        return Unit.Value;
    }
}