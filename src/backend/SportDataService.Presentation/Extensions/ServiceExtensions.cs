using System.Text;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NLog;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Application.DTO.MappingProfiles;
using SportDataService.Application.UseCases.Match;
using SportDataService.Application.UseCases.MatchResult;
using SportDataService.Application.UseCases.Team;
using SportDataService.Application.UseCases.Tournament;
using SportDataService.Application.UseCases.TournamentResult;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Infrastructure.Configs;
using SportDataService.Infrastructure.Repository;
using SportDataService.Infrastructure.Services;
using SportDataService.Infrastructure.Services.DataCollection;
using SportDataService.Infrastructure.Services.DataCollection.Abstractions;
using SportDataService.Infrastructure.Services.DataCollection.Implementations;
using SportDataService.Infrastructure.Services.Hangfire;
using SportDataService.Infrastructure.Utility;
using SportDataService.Presentation.Utility;

namespace SportDataService.Presentation.Extensions;

public static class ServiceExtensions
{
    public static void AddDockerSecrets(this IConfigurationBuilder config)
    {
        const string secretsPath = "/run/secrets/";
        if (Directory.Exists(secretsPath))
        {
            foreach (var file in Directory.GetFiles(secretsPath))
            {
                config.AddKeyPerFile(file, optional: true);
            }
        }
    }

    public static void AddAppSettings(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SportDataDbSettings>(configuration.GetSection("DatabaseSettings"));
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<DataCollectionServiceSettings>(configuration.GetSection("DataCollectionServiceSettings"));
        services.Configure<HangfireSettings>(configuration.GetSection("HangfireSettings"));
    }

    public static void ConfigureNLog(this IServiceCollection services)
    {
        const string configPath = "/app/Properties/nlog.config";
        if (File.Exists(configPath))
        {
            LogManager.Setup().LoadConfigurationFromFile(configPath);
            return;
        }

        throw new FileNotFoundException($"NLog config not found at: {configPath}");
    }

    public static void ConfigureLoggerService(this IServiceCollection services) =>
        services.AddSingleton<ILoggerService, LoggerService>();

    public static void ConfigureDataCollectionService(this IServiceCollection services)
    {
        services.AddHttpClient<IApiDataService, ApiDataService>();
        services.AddScoped<IPrematchDataParser, PrematchDataParser>();
        services.AddScoped<IResultsDataParser, ResultsDataParser>();
        services.AddScoped<IDataCollectionService, DataCollectionService>();
    }
    public static void ConfigureBackgroundJobService(this IServiceCollection services) =>
        services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();

    public static void ConfigureBackgroundJobExecutor(this IServiceCollection services) =>
        services.AddScoped<IBackgroundJobExecutor, HangfireJobExecutor>();

    public static void ConfigureUseCases(this IServiceCollection services)
    {
        // tournament
        services.AddScoped<IGetAllTournamentsUseCase, GetAllTournamentsUseCase>();
        services.AddScoped<IGetTournamentByIdUseCase, GetTournamentByIdUseCase>();
        services.AddScoped<IGetTournamentByTournamentIdUseCase, GetTournamentByTournamentIdUseCase>();
        services.AddScoped<IRefreshTournamentsUseCase, RefreshTournamentsUseCase>();

        // team
        services.AddScoped<IGetAllTeamsUseCase, GetAllTeamsUseCase>();
        services.AddScoped<IGetTeamByIdUseCase, GetTeamByIdUseCase>();
        services.AddScoped<IGetTeamByTeamIdUseCase, GetTeamByTeamIdUseCase>();

        // match
        services.AddScoped<IGetAllMatchesUseCase, GetAllMatchesUseCase>();
        services.AddScoped<IGetMatchByIdUseCase, GetMatchByIdUseCase>();
        services.AddScoped<IGetMatchByMatchIdUseCase, GetMatchByMatchIdUseCase>();

        // tournament results
        services.AddScoped<IGetAllTournamentResultsUseCase, GetAllTournamentResultsUseCase>();
        services.AddScoped<IGetTournamentResultByIdUseCase, GetTournamentResultByIdUseCase>();
        services
            .AddScoped<IGetTournamentResultByResultIdUseCase,
                GetTournamentResultByResultIdUseCase>();
        services.AddScoped<IRefreshTournamentResultsUseCase, RefreshTournamentResultsUseCase>();

        // match results
        services.AddScoped<IGetAllMatchResultsUseCase, GetAllMatchResultsUseCase>();
        services.AddScoped<IGetMatchResultByIdUseCase, GetMatchResultByIdUseCase>();
        services.AddScoped<IGetMatchResultByResultIdUseCase, GetMatchResultByResultIdUseCase>();
    }

    public static void ConfigureMongoDbMappings(this IServiceCollection services)
    {
        MongoDbMappingConfig.ConfigureMappings();
    }

    public static void ConfigureMongoDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = configuration.GetConnectionString("DbConnection");

            var clientSettings = MongoClientSettings.FromConnectionString(connectionString);
            var settings = configuration.GetSection("SportDataDbSettings").Get<SportDataDbSettings>()!;
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

            return new MongoClient(clientSettings);
        });

        services.AddSingleton<IMongoClient>(_ =>
        {
            var connectionString = configuration.GetConnectionString("HangfireDbConnection");
            return new MongoClient(connectionString);
        });
    }

    public static void AddSportDataDb(this IServiceCollection services)
    {
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<IOptions<SportDataDbSettings>>().Value;
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

        services.AddScoped<ITournamentResultRepository>(sp =>
            new TournamentResultsRepository(sp.GetRequiredService<IMongoDatabase>()));

        services.AddScoped<IMatchResultRepository>(sp =>
            new MatchResultRepository(sp.GetRequiredService<IMongoDatabase>()));
    }

    public static void ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(
            cfg =>
            {
                cfg.AddProfile<GetTournamentMappingProfile>();
                cfg.AddProfile<GetTeamMappingProfile>();
                cfg.AddProfile<GetMatchMappingProfile>();
                cfg.AddProfile<GetSubScoreMappingProfile>();
                cfg.AddProfile<GetMatchResultMappingProfile>();
                cfg.AddProfile<GetMatchEventResultMappingProfile>();
                cfg.AddProfile<GetTournamentResultMappingProfile>();
            },
            AppDomain.CurrentDomain.GetAssemblies());
    }

    public static void ConfigureApiBehaviorOptions(this IServiceCollection services) =>
        services.Configure<ApiBehaviorOptions>(opt => { opt.SuppressModelStateInvalidFilter = true; });

    public static void ConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer();

        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!)),
                    ClockSkew = TimeSpan.Zero,
                };
            });
    }

    public static void AddAuthorizationPolicies(this IServiceCollection services) =>
        services.AddAuthorization(options =>
        {
            options.AddPolicy(
                AuthorizationPolicies.AdministratorOnly,
                policy => policy.RequireRole(UserRoles.Administrator));
        });

    public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("HangfireDbConnection");
        var settings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>()!;

        services.AddHangfire(config => config
            .UseMongoStorage(
                connectionString,
                settings.DatabaseName,
                new MongoStorageOptions
                {
                    CheckConnection = true,
                    CheckQueuedJobsStrategy = CheckQueuedJobsStrategy.TailNotificationsCollection,
                    MigrationOptions = new MongoMigrationOptions
                    {
                        MigrationStrategy = new MigrateMongoMigrationStrategy(),
                        BackupStrategy = new CollectionMongoBackupStrategy(),
                    },
                })
            .UseFilter(new AutomaticRetryAttribute { Attempts = 3 }));

        services.AddHangfireServer(options =>
        {
            options.ServerName = "SportData.Background";
            options.Queues = new[] { "default", "critical" };
            options.WorkerCount = 1;
        });
    }
}