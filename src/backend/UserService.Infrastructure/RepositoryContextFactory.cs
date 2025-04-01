using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;
using UserService.Domain.Models;
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
        var presentationPath = Path.Combine(presentationAssemblyPath, @"..\..\..\..\", @"UserService.Presentation\Properties");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(presentationPath)
            .AddYamlFile("secrets.yaml")
            .Build();

        var databaseSettings = configuration.GetSection("DatabaseSettings").Get<DatabaseSettings>();

        var optionsBuilder = new DbContextOptionsBuilder<RepositoryContext>();
        optionsBuilder.UseNpgsql(databaseSettings.ConnectionString);

        return new RepositoryContext(optionsBuilder.Options);
    }
}