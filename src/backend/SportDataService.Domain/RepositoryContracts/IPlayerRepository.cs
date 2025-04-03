using SportDataService.Domain.Models;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IPlayerRepository : IRepositoryBase<Player>
{
    public Task<PagedList<Player>> FindAllPlayersAsync(PlayerParameters playerParameters, CancellationToken cancellationToken);
}