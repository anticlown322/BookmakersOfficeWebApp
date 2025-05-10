using SportDataService.Domain.Models.Common;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ITeamRepository : IRepositoryBase<Team>
{
    Task<PagedList<Team>> FindAllTeamsAsync(TeamParameters teamParameters, CancellationToken cancellationToken);
    Task<Team?> GetTeamByTeamIdAsync(string teamId, CancellationToken ct);
}