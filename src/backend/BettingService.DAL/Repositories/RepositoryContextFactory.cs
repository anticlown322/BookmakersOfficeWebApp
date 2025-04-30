using BettingService.DAL.Models.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using YamlDotNet.Serialization;

namespace BettingService.DAL.Repositories;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var basePath = Directory.GetCurrentDirectory();
        var apiPath = Path.GetFullPath(Path.Combine(basePath, @"..\BettingService.API\Properties"));
        var jsonFilePath = Path.Combine(apiPath, "secrets.json");
        if (!File.Exists(jsonFilePath))
        {
            throw new FileNotFoundException($"secrets.json not found at: {jsonFilePath}");
        }

        var settings = File.ReadAllText(jsonFilePath);
        var deserializer = new DeserializerBuilder().Build();

        var databaseSettings = deserializer.Deserialize<RootSettings>(settings).DatabaseSettings;
        var connectionString = databaseSettings.ConnectionString;
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string is missing in secrets.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new RepositoryContext(optionsBuilder.Options);
    }
}