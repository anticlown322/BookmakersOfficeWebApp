using MongoDB.Driver;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public sealed class TeamRepository : MongoRepositoryBase<Team>, ITeamRepository
{
    public TeamRepository(IMongoDatabase database)
        : base(database, "teams")
    {
    }
}