using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;

namespace BettingService.DAL.Repositories.Implementations;

public class PayoutRepository(
    RepositoryContext context)
    : RepositoryBase<Payout>(context),
        IPayoutRepository
{
    public async Task<Payout?> GetByBetIdAsync(Guid betId, CancellationToken cancellationToken)
    {
        var payout = await FindByConditionAsync(p => p.BetId == betId, trackChanges: false, cancellationToken);
        
        return payout.SingleOrDefault();
    }
}