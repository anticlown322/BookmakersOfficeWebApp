using MongoDB.Driver;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbCollectionsConfig(IMongoDatabase database)
{
    public async Task CreateCollectionsIfNotExistsAsync()
    {
        var collections = await (await database.ListCollectionNamesAsync()).ToListAsync();
        var requiredCollections = new[] { "matches", "events", "odds", "leagues", "teams", "players" };
        foreach (var collection in requiredCollections)
        {
            if (!collections.Contains(collection))
            {
                await database.CreateCollectionAsync(collection);
            }
        }
    }
}