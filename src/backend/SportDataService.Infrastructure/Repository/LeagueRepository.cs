using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public sealed class LeagueRepository : MongoRepositoryBase<League>, ILeagueRepository
{
    public LeagueRepository(IMongoDatabase database)
        : base(database, "leagues")
    {
    }
}