using SportDataService.Domain.Models;
using SportDataService.Domain.Models.Tournaments;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ITeamRepository : IRepositoryBase<Team>
{
    Task<PagedList<Team>> FindAllTeamsAsync(TeamParameters teamParameters, CancellationToken cancellationToken);
    Task<Team?> GetByTeamIdAsync(string teamId, CancellationToken ct);
}