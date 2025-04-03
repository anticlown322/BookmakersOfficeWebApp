using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class MatchRepository : MongoRepositoryBase<Match>, IMatchRepository
{
    public MatchRepository(IMongoDatabase database)
        : base(database, "matches")
    {
    }

    public async Task<PagedList<Match>> FindAllMatchesAsync(MatchParameters matchParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var matches = await FindAllAsync(cancellationToken);

        var orderedMatches = matches.OrderBy(p => p.CreatedAt);

        var pagedMatches = orderedMatches
            .Skip((matchParameters.PageNumber - 1) * matchParameters.PageSize)
            .Take(matchParameters.PageSize)
            .ToList();

        var totalCount = orderedMatches.Count();

        return new PagedList<Match>(
            pagedMatches,
            totalCount,
            matchParameters.PageNumber,
            matchParameters.PageSize);
    }
}