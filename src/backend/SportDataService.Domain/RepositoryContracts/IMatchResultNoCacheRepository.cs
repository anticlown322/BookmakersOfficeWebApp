using SportDataService.Domain.Models.Results;

namespace SportDataService.Domain.RepositoryContracts;

public interface IMatchResultNoCacheRepository : INoCacheRepositoryBase<MatchResult>
{
    Task<MatchResult?> GetMatchResultByMatchResultIdAsync(string matchResultId, CancellationToken ct);
}