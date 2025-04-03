using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public sealed class EventRepository : MongoRepositoryBase<Event>, IEventRepository
{
    public EventRepository(IMongoDatabase database)
        : base(database, "events")
    {
    }

    public async Task<PagedList<Event>> FindAllEventsAsync(EventParameters eventParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var events = await FindAllAsync(cancellationToken);

        var orderedEvents = events.OrderBy(p => p.MatchId);

        var pagedEvents = orderedEvents
            .Skip((eventParameters.PageNumber - 1) * eventParameters.PageSize)
            .Take(eventParameters.PageSize)
            .ToList();

        var totalCount = orderedEvents.Count();

        return new PagedList<Event>(
            pagedEvents,
            totalCount,
            eventParameters.PageNumber,
            eventParameters.PageSize);
    }
}