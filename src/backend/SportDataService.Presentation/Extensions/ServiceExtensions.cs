using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NLog;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.MappingProfiles;
using SportDataService.Application.DTO.MappingProfiles.Player;
using SportDataService.Application.DTO.MappingProfiles.Team;
using SportDataService.Application.UseCases.Match;
using SportDataService.Application.UseCases.Player;
using SportDataService.Application.UseCases.Team;
using SportDataService.Domain.Models;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Infrastructure.Configs;
using SportDataService.Infrastructure.Repository;
using SportDataService.Infrastructure.Services;

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
    }

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerService, LoggerService>();

    public static void ConfigureUseCases(this IServiceCollection services)
    {
        // match
        services.AddScoped<IGetAllMatchesUseCase, GetAllMatchesUseCase>();

        // player
        services.AddScoped<IGetAllPlayersUseCase, GetAllPlayersUseCase>();
        services.AddScoped<ICreatePlayerUseCase, CreatePlayerUseCase>();
        services.AddScoped<IGetPlayerByIdUseCase, GetPlayerByIdUseCase>();
        services.AddScoped<IUpdatePlayerUseCase, UpdatePlayerUseCase>();
        services.AddScoped<IDeletePlayerUseCase, DeletePlayerUseCase>();

        // team
        services.AddScoped<IGetAllTeamsUseCase, GetAllTeamsUseCase>();
        services.AddScoped<ICreateTeamUseCase, CreateTeamUseCase>();
        services.AddScoped<IGetTeamByIdUseCase, GetTeamByIdUseCase>();
        services.AddScoped<IUpdateTeamUseCase, UpdateTeamUseCase>();
        services.AddScoped<IDeleteTeamUseCase, DeleteTeamUseCase>();
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
        services.AddScoped<IMatchRepository>(sp =>
            new MatchRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<IEventRepository>(sp =>
            new EventRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<ILeagueRepository>(sp =>
            new LeagueRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<IPlayerRepository>(sp =>
            new PlayerRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<ITeamRepository>(sp =>
            new TeamRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<IOddsRepository, OddsRepository>();
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(
            cfg =>
        {
            cfg.AddProfile<GetMatchesMappingProfile>();

            // player
            cfg.AddProfile<GetPlayerMappingProfile>();
            cfg.AddProfile<CreatePlayerMappingProfile>();
            cfg.AddProfile<UpdatePlayerMappingProfile>();

            // team
            cfg.AddProfile<GetTeamMappingProfile>();
            cfg.AddProfile<CreateTeamMappingProfile>();
            cfg.AddProfile<UpdateTeamMappingProfile>();
        }, AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void AddValidators(this IServiceCollection services)
    {
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });
}