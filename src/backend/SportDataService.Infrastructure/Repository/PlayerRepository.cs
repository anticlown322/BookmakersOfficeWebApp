using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public sealed class PlayerRepository : MongoRepositoryBase<Player>, IPlayerRepository
{
    public PlayerRepository(IMongoDatabase database)
        : base(database, "players")
    {
    }
}