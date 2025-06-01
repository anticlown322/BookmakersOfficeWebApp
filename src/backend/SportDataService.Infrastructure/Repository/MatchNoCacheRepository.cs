using MongoDB.Driver;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public class MatchNoCacheRepository(IMongoDatabase database)
    : BaseNoCacheRepository<Match>(database, "matches"), IMatchNoCacheRepository;