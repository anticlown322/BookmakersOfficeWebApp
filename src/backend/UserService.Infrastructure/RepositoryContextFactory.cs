using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using UserService.Infrastructure.AppSettings;
using UserService.Infrastructure.Repository;

namespace UserService.Infrastructure;

/// <summary>
/// For migrations
/// </summary>
public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var presentationAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var presentationPath = Path.Combine(presentationAssemblyPath, @"..\..\..\..\", "UserService.Presentation");

        // Загружаем конфигурацию из appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(presentationPath)
            .AddJsonFile("appsettings.json")
            .Build();

        var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

        var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>();
        optionsBuilder.UseNpgsql(databaseSettings.ConnectionString);

        return new RepositoryContext(optionsBuilder.Options);
    }
}