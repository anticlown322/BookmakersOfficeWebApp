using SportDataService.Domain.Models;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ITeamRepository : IRepositoryBase<Team>
{
    public Task<PagedList<Team>> FindAllTeamsAsync(TeamParameters teamParameters, CancellationToken cancellationToken);
}