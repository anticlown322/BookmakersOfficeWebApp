using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IMatchResultRepository : ICachedRepository<MatchResult>
{
    public Task<PagedList<MatchResult>> FindAllMatchResultsAsync(MatchResultParameters matchResultParameters, CancellationToken cancellationToken);
    Task<MatchResult?> GetMatchResultByMatchResultIdAsync(string matchResultId, CancellationToken ct);
}