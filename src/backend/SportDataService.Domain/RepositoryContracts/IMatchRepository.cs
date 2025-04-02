using SportDataService.Domain.Models;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IMatchRepository : IRepositoryBase<Match>
{
    public Task<PagedList<Match>> FindAllMatchesAsync(MatchParameters matchParameters, CancellationToken cancellationToken);
}