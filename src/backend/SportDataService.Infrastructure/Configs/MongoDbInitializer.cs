using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace SportDataService.Infrastructure.Configs;

public class MongoDbInitializer(
    IMongoDatabase database,
    ILogger<MongoDbInitializer> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Initializing MongoDB...");

        var config = new MongoDbConfig(database);
        await config.ConfigureAsync();

        logger.LogInformation("MongoDB initialized");
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}