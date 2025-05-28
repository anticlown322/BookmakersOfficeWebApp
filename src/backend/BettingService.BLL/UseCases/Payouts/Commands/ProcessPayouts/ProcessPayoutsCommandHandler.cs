using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Payouts.Commands.ProcessPayouts;

public class ProcessPayoutsCommandHandler(
    IPayoutRepository payoutRepository,
    UserGrpcService.UserGrpcServiceClient userGrpcClient,
    ILogger<ProcessPayoutsCommandHandler> logger)
    : IRequestHandler<ProcessPayoutsCommand, Unit>
{
    public async Task<Unit> Handle(ProcessPayoutsCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Start processing pending payouts...");

        var pendingPayouts = await payoutRepository.FindByConditionAsync(
            p => p.Status == PayoutStatus.Pending,
            trackChanges: true,
            cancellationToken);

        if (!pendingPayouts.Any())
        {
            logger.LogInformation("No pending payouts found");

            return Unit.Value;
        }

        var payoutsByUser = pendingPayouts.GroupBy(p => p.Username);

        logger.LogInformation("Updating payouts status...");

        foreach (var userGroup in payoutsByUser)
        {
            var username = userGroup.Key;
            var totalAmount = userGroup.Sum(p => p.Amount);

            logger.LogInformation($"Updating payout status for {username}");

            try
            {
                var updateResponse = await userGrpcClient.UpdateUserBalanceAsync(
                    new UpdateUserBalanceRequest
                    {
                        Username = username,
                        Amount = (double)totalAmount,
                    },
                    cancellationToken: cancellationToken);

                if (!updateResponse.Success)
                {
                    logger.LogWarning($"Failed to update user balance for {username}");
                    await MarkPayoutsAsFailed(userGroup, "Balance update failed", cancellationToken);
                    continue;
                }

                await MarkPayoutsAsCompleted(userGroup, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, $"Failed to update user balance for {username}");
                await MarkPayoutsAsFailed(userGroup, ex.Message, cancellationToken);
            }
        }

        logger.LogInformation("Finished processing pending payouts");

        return Unit.Value;
    }

    private async Task MarkPayoutsAsCompleted(
        IEnumerable<Payout> payouts,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        foreach (var payout in payouts)
        {
            payout.Status = PayoutStatus.Completed;
            payout.ProcessedAt = now;
            payoutRepository.Update(payout);
        }

        await payoutRepository.SaveAsync(cancellationToken);
    }

    private async Task MarkPayoutsAsFailed(
        IEnumerable<Payout> payouts,
        string errorReason,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        foreach (var payout in payouts)
        {
            payout.Status = PayoutStatus.Failed;
            payout.ProcessedAt = now;
            payout.ErrorReason = errorReason;
            payoutRepository.Update(payout);
        }

        await payoutRepository.SaveAsync(cancellationToken);
    }
}