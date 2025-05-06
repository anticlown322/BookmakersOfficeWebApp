using MongoDB.Driver;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class MatchResultRepository : MongoRepositoryBase<MatchResult>, IMatchResultRepository
{
    public MatchResultRepository(IMongoDatabase database)
        : base(database, "matchResults")
    {
    }

    public async Task<PagedList<MatchResult>> FindAllMatchResultsAsync(MatchResultParameters matchResultParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var matchResults = await FindAllAsync(cancellationToken);

        var orderedMatchResults = matchResults.OrderBy(p => p.ResultTime);

        var pagedMatchResults = orderedMatchResults
            .Skip((matchResultParameters.PageNumber - 1) * matchResultParameters.PageSize)
            .Take(matchResultParameters.PageSize)
            .ToList();

        var totalCount = orderedMatchResults.Count();

        return new PagedList<MatchResult>(
            pagedMatchResults,
            totalCount,
            matchResultParameters.PageNumber,
            matchResultParameters.PageSize);
    }

    public async Task<MatchResult?> GetMatchResultByMatchResultIdAsync(string matchResultId, CancellationToken ct)
    {
        var filter = Builders<MatchResult>.Filter.Eq(t => t.MatchResultId, matchResultId);

        ct.ThrowIfCancellationRequested();

        return await Collection.Find(filter).FirstOrDefaultAsync(ct);
    }
}