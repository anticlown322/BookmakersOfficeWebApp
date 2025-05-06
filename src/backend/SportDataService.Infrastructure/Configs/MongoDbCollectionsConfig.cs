using MongoDB.Driver;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbCollectionsConfig(IMongoDatabase database)
{
    public async Task CreateCollectionsIfNotExistsAsync()
    {
        var collections = await (await database.ListCollectionNamesAsync()).ToListAsync();
        var requiredCollections = new[] { "teams", "matches", "tournaments", "matchResults", "tournamentResults" };
        foreach (var collection in requiredCollections)
        {
            if (!collections.Contains(collection))
            {
                await database.CreateCollectionAsync(collection);
            }
        }
    }
}