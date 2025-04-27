using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IMatchRepository : IRepositoryBase<Match>
{
    public Task<PagedList<Match>> FindAllMatchesAsync(MatchParameters matchParameters, CancellationToken cancellationToken);
    Task<Match?> GetMatchByMatchIdAsync(string matchId, CancellationToken ct);
}