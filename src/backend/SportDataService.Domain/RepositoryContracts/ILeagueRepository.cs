using SportDataService.Domain.Models;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ILeagueRepository : IRepositoryBase<League>
{
    public Task<PagedList<League>> FindAllLeaguesAsync(LeagueParameters teamParameters, CancellationToken cancellationToken);
}