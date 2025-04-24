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
        var yamlPath = Path.Combine(apiPath, "secrets.yaml");
        if (!File.Exists(yamlPath))
        {
            throw new FileNotFoundException($"secrets.yaml not found at: {yamlPath}");
        }

        var yamlContent = File.ReadAllText(yamlPath);
        var deserializer = new DeserializerBuilder().Build();

        var databaseSettings = deserializer.Deserialize<RootSettings>(yamlContent).DatabaseSettings;
        var connectionString = databaseSettings.ConnectionString;
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Connection string is missing in secrets.yaml");
        }

        var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new RepositoryContext(optionsBuilder.Options);
    }
}