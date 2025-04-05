using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class OddsRepository : MongoRepositoryBase<Odds>, IOddsRepository
{
    public OddsRepository(IMongoDatabase database)
        : base(database, "odds")
    {
    }

    public async Task<PagedList<Odds>> FindAllOddsAsync(OddsParameters oddsParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var odds = await FindAllAsync(cancellationToken);

        var orderedOdds = odds.OrderBy(p => p.MatchId);

        var pagedOdds = orderedOdds
            .Skip((oddsParameters.PageNumber - 1) * oddsParameters.PageSize)
            .Take(oddsParameters.PageSize)
            .ToList();

        var totalCount = orderedOdds.Count();

        return new PagedList<Odds>(
            pagedOdds,
            totalCount,
            oddsParameters.PageNumber,
            oddsParameters.PageSize);
    }
}