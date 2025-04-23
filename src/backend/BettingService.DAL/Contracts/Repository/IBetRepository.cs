using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.DAL.Contracts.Repository;

public interface IBetRepository : IRepositoryBase<Bet>
{
    Task<PagedList<Bet>> GetAllBetsAsync(BetParameters betParameters, CancellationToken cancellationToken);

    Task<PagedList<Bet>> GetUserBetsAsync(
        BetParameters betParameters,
        string username,
        CancellationToken cancellationToken);

    Task<Bet?> GetByIdAsync(Guid betId, CancellationToken cancellationToken);
}