using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class LeagueRepository : MongoRepositoryBase<League>, ILeagueRepository
{
    public LeagueRepository(IMongoDatabase database)
        : base(database, "leagues")
    {
    }

    public async Task<PagedList<League>> FindAllLeaguesAsync(LeagueParameters leagueParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var leagues = await FindAllAsync(cancellationToken);

        var orderedLeagues = leagues.OrderBy(p => p.Name);

        var pagedLeagues = orderedLeagues
            .Skip((leagueParameters.PageNumber - 1) * leagueParameters.PageSize)
            .Take(leagueParameters.PageSize)
            .ToList();

        var totalCount = orderedLeagues.Count();

        return new PagedList<League>(
            pagedLeagues,
            totalCount,
            leagueParameters.PageNumber,
            leagueParameters.PageSize);
    }
}