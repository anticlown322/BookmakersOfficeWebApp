using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.DAL.Repositories.Implementations;

public class PayoutRepository(RepositoryContext context)
    : RepositoryBase<Payout>(context),
        IPayoutRepository
{
    public async Task<Payout?> GetByIdAsync(Guid payoutId, CancellationToken cancellationToken)
    {
        var payout = await FindByConditionAsync(b => b.Id == payoutId, trackChanges: false, cancellationToken);

        return payout.SingleOrDefault();
    }

    public async Task<Payout?> GetByBetIdAsync(Guid payoutId, CancellationToken cancellationToken)
    {
        var payout = await FindByConditionAsync(p => p.BetId == payoutId, trackChanges: false, cancellationToken);

        return payout.SingleOrDefault();
    }

    public async Task<PagedList<Payout>> GetAllPayoutsAsync(
        PayoutParameters payoutParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payouts = await FindAllAsync(false, cancellationToken);

        var orderedPayouts = payouts.OrderBy(p => p.ProcessedAt);

        var pagedPayouts = orderedPayouts
            .Skip((payoutParameters.PageNumber - 1) * payoutParameters.PageSize)
            .Take(payoutParameters.PageSize)
            .ToList();

        var totalCount = orderedPayouts.Count();

        return new PagedList<Payout>(
            pagedPayouts,
            totalCount,
            payoutParameters.PageNumber,
            payoutParameters.PageSize);
    }

    public async Task<PagedList<Payout>> GetUserPayoutsAsync(
        PayoutParameters payoutParameters,
        string username,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var payouts = await FindByConditionAsync(b => b.Username == username, trackChanges: false, cancellationToken);

        var orderedPayouts = payouts.OrderBy(p => p.CreatedAt);

        var pagedPayouts = orderedPayouts
            .Skip((payoutParameters.PageNumber - 1) * payoutParameters.PageSize)
            .Take(payoutParameters.PageSize)
            .ToList();

        var totalCount = orderedPayouts.Count();

        return new PagedList<Payout>(
            pagedPayouts,
            totalCount,
            payoutParameters.PageNumber,
            payoutParameters.PageSize);
    }
}