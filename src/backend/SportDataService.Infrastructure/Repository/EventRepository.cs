using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public sealed class EventRepository : MongoRepositoryBase<Event>, IEventRepository
{
    public EventRepository(IMongoDatabase database)
        : base(database, "events")
    {
    }
}