using MongoDB.Driver;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public class MatchResultNoCacheRepository(IMongoDatabase database)
    : BaseNoCacheRepository<MatchResult>(database, "matchResults"), IMatchResultNoCacheRepository
{
    public async Task<MatchResult?> GetMatchResultByMatchResultIdAsync(string matchResultId, CancellationToken ct)
    {
        var filter = Builders<MatchResult>.Filter.Eq(t => t.MatchResultId, matchResultId);

        ct.ThrowIfCancellationRequested();

        return await Collection.Find(filter).FirstOrDefaultAsync(ct);
    }
}