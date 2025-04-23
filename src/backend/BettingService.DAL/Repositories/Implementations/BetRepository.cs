using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.DAL.Repositories.Implementations;

public class BetRepository(RepositoryContext context)
    : RepositoryBase<Bet>(context),
        IBetRepository
{
    public async Task<PagedList<Bet>> GetAllBetsAsync(
        BetParameters betParameters,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var bets = await FindAllAsync(false, cancellationToken);

        var orderedBets = bets.OrderBy(p => p.CreatedAt);

        var pagedBets = orderedBets
            .Skip((betParameters.PageNumber - 1) * betParameters.PageSize)
            .Take(betParameters.PageSize)
            .ToList();

        var totalCount = orderedBets.Count();

        return new PagedList<Bet>(
            pagedBets,
            totalCount,
            betParameters.PageNumber,
            betParameters.PageSize);
    }

    public async Task<PagedList<Bet>> GetUserBetsAsync(
        BetParameters betParameters,
        string username,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var bets = await FindByConditionAsync(b => b.Username == username, trackChanges: false, cancellationToken);

        var orderedBets = bets.OrderBy(p => p.CreatedAt);

        var pagedBets = orderedBets
            .Skip((betParameters.PageNumber - 1) * betParameters.PageSize)
            .Take(betParameters.PageSize)
            .ToList();

        var totalCount = orderedBets.Count();

        return new PagedList<Bet>(
            pagedBets,
            totalCount,
            betParameters.PageNumber,
            betParameters.PageSize);
    }

    public async Task<Bet?> GetByIdAsync(Guid betId, CancellationToken cancellationToken)
    {
        var bet = await FindByConditionAsync(b => b.Id == betId, trackChanges: false, cancellationToken);

        return bet.SingleOrDefault();
    }
}