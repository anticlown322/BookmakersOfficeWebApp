using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.DAL.Contracts.Repository;

public interface IPayoutRepository : IRepositoryBase<Payout>
{
    Task<Payout?> GetByIdAsync(Guid payoutId, CancellationToken cancellationToken);
    Task<Payout?> GetByBetIdAsync(Guid payoutId, CancellationToken cancellationToken);
    Task<PagedList<Payout>> GetAllPayoutsAsync(PayoutParameters payoutParameters, CancellationToken cancellationToken);

    Task<PagedList<Payout>> GetUserPayoutsAsync(
        PayoutParameters payoutParameters,
        string username,
        CancellationToken cancellationToken);
}