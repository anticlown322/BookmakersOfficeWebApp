using SportDataService.Domain.Models;
using SportDataService.Domain.Models.Tournaments;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IMatchRepository : IRepositoryBase<Match>
{
    public Task<PagedList<Match>> FindAllMatchesAsync(MatchParameters matchParameters, CancellationToken cancellationToken);
    Task<Match?> GetByMatchIdAsync(string matchId, CancellationToken ct);
}