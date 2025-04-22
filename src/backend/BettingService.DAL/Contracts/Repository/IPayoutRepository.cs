using BettingService.DAL.Models.Entities;

namespace BettingService.DAL.Contracts.Repository;

public interface IPayoutRepository : IRepositoryBase<Payout>
{
    Task<Payout?> GetByBetIdAsync(Guid betId, CancellationToken cancellationToken);
}