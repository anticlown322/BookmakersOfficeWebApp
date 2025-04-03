using SportDataService.Domain.Models;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface IEventRepository : IRepositoryBase<Event>
{
    public Task<PagedList<Event>> FindAllEventsAsync(EventParameters eventParameters, CancellationToken cancellationToken);
}