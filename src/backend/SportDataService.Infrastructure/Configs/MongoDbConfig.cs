using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using SportDataService.Domain.Models;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbConfig(IMongoDatabase database)
{
    private readonly MongoDbCollectionsConfig _collectionsConfig = new (database);
    private readonly MongoDbIndexesConfig _indexesConfig = new (database);
    private readonly MongoDbValidatorsConfig _validatorsConfig = new (database);

    public async Task ConfigureAsync()
    {
        await _collectionsConfig.CreateCollectionsIfNotExistsAsync();
        await _indexesConfig.CreateIndexesAsync();
        await _validatorsConfig.ConfigureSchemaValidationsAsync();
    }
}