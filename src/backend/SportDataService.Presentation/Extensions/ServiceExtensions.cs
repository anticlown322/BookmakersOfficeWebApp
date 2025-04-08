using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.MappingProfiles;
using SportDataService.Application.UseCases.Match;
using SportDataService.Application.UseCases.Team;
using SportDataService.Application.UseCases.Tournament;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Infrastructure.Configs;
using SportDataService.Infrastructure.Repository;
using SportDataService.Infrastructure.Services;
using SportDataService.Infrastructure.Services.DataCollection;

namespace SportDataService.Presentation.Extensions;

public static class ServiceExtensions
{
    public static IConfigurationBuilder AddSecretsYaml(this IConfigurationBuilder configurationBuilder)
    {
        var path = Directory.GetCurrentDirectory() + @"\Properties";

        return configurationBuilder
            .SetBasePath(path)
            .AddYamlFile("secrets.yaml", optional: true, reloadOnChange: true);
    }

    public static void ConfigureNLog(this IServiceCollection services)
    {
        var configFilePath = Path.Combine(Directory.GetCurrentDirectory(), @"Properties\nlog.config");

        if (!File.Exists(configFilePath))
        {
            throw new FileNotFoundException($"NLog configuration file not found: {configFilePath}");
        }

        LogManager.Setup().LoadConfigurationFromFile(configFilePath);
    }

    public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseSettings>(configuration.GetSection("DatabaseSettings"));
        services.Configure<DataCollectionServiceSettings>(configuration.GetSection("DataCollectionServiceSettings"));
    }

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerService, LoggerService>();

    public static void ConfigureDataCollectionService(this IServiceCollection services) =>
        services.AddSingleton<IDataCollectionService, DataCollectionService>();

    public static void ConfigureUseCases(this IServiceCollection services)
    {
        // tournament
        services.AddScoped<IGetAllTournamentsUseCase, GetAllTournamentsUseCase>();
        services.AddScoped<IGetTournamentByIdUseCase, GetTournamentByIdUseCase>();
        services.AddScoped<IGetTournamentByTournamentIdUseCase, GetTournamentByTournamentIdUseCase>();
        services.AddScoped<IForceTournamentRefresh, ForceTournamentRefresh>();

        // team
        services.AddScoped<IGetAllTeamsUseCase, GetAllTeamsUseCase>();
        services.AddScoped<IGetTeamByIdUseCase, GetTeamByIdUseCase>();
        services.AddScoped<IGetTeamByTeamIdUseCase, GetTeamByTeamIdUseCase>();

        // match
        services.AddScoped<IGetAllMatchesUseCase, GetAllMatchesUseCase>();
        services.AddScoped<IGetMatchByIdUseCase, GetMatchByIdUseCase>();
        services.AddScoped<IGetMatchByMatchIdUseCase, GetMatchByMatchIdUseCase>();
    }

    public static void ConfigureMongoDbMappings(this IServiceCollection services)
    {
        MongoDbMappingConfig.ConfigureMappings();
    }

    public static void ConfigureMongoDbContext(this IServiceCollection services)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            return new MongoClient(clientSettings);
        });

        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
            return client.GetDatabase(settings.DatabaseName);
        });
    }

    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITournamentRepository>(sp =>
            new TournamentsRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<ITeamRepository>(sp =>
            new TeamRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<IMatchRepository>(sp =>
            new MatchRepository(sp.GetRequiredService<IMongoDatabase>()));
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(
            cfg =>
        {
            cfg.AddProfile<GetTournamentMappingProfile>();
            cfg.AddProfile<GetTeamMappingProfile>();
            cfg.AddProfile<GetMatchMappingProfile>();
        }, AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}