using SportDataService.Domain.Models.Common;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ITeamRepository : ICachedRepository<Team>
{
    Task<PagedList<Team>> FindAllTeamsAsync(TeamParameters teamParameters, CancellationToken ct);
    Task<Team?> GetTeamByTeamIdAsync(string teamId, CancellationToken ct);
}