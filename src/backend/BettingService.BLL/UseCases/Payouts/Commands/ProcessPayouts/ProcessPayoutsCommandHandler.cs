using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Commands.ProcessPayouts;

public class ProcessPayoutsCommandHandler(
    IPayoutRepository payoutRepository,
    UserGrpcService.UserGrpcServiceClient userGrpcClient)
    : IRequestHandler<ProcessPayoutsCommand, Unit>
{
    public async Task<Unit> Handle(ProcessPayoutsCommand request, CancellationToken cancellationToken)
    {
        var pendingPayouts = await payoutRepository.FindByConditionAsync(
            p => p.Status == PayoutStatus.Pending,
            trackChanges: true,
            cancellationToken);

        if (!pendingPayouts.Any())
        {
            return Unit.Value;
        }

        var payoutsByUser = pendingPayouts.GroupBy(p => p.Username);

        foreach (var userGroup in payoutsByUser)
        {
            var username = userGroup.Key;
            var totalAmount = userGroup.Sum(p => p.Amount);

            try
            {
                var updateResponse = await userGrpcClient.UpdateUserBalanceAsync(
                    new UpdateUserBalanceRequest
                    {
                        Username = username,
                        Amount = (double)totalAmount
                    },
                    cancellationToken: cancellationToken);

                if (!updateResponse.Success)
                {
                    await MarkPayoutsAsFailed(userGroup, "Balance update failed", cancellationToken);
                    continue;
                }

                await MarkPayoutsAsCompleted(userGroup, cancellationToken);
            }
            catch (Exception ex)
            {
                await MarkPayoutsAsFailed(userGroup, ex.Message, cancellationToken);
            }
        }

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