using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public sealed class OddsRepository : MongoRepositoryBase<Odds>, IOddsRepository
{
    public OddsRepository(IMongoDatabase database)
        : base(database, "odds")
    {
    }
}