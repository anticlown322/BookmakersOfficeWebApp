using SportDataService.Domain.Models;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IOddsRepository : IRepositoryBase<Odds>
{
    public Task<PagedList<Odds>> FindAllOddsAsync(OddsParameters eventParameters, CancellationToken cancellationToken);
}