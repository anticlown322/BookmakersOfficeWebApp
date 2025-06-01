using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.GrpcService.Exceptions;
using SportDataService.GrpcService.Services;
using SportDataService.Infrastructure.Configs;
using SportDataService.Infrastructure.Repository;
using SportDataService.Infrastructure.Services.EventBus.Kafka.Abstractions;
using SportDataService.Infrastructure.Services.EventBus.Kafka.Implementations;

namespace SportDataService.GrpcService.Extensions;

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
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
    }
    
    public static void ConfigureGrpc(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.EnableDetailedErrors = true;
            options.Interceptors.Add<ExceptionInterceptor>();
        });
        
        services.AddScoped<SportDataGrpcService>();
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
            var settings = configuration.GetSection("DatabaseSettings").Get<SportDataDbSettings>()!;
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
            
            return new MongoClient(clientSettings);
        });
    }

    public static void AddSportDataDb(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var settings = configuration.GetSection("DatabaseSettings").Get<SportDataDbSettings>()!;   
            var client = sp.GetRequiredService<IMongoClient>();
            return client.GetDatabase(settings.DatabaseName);
        });
    }
    
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IMatchNoCacheRepository, MatchNoCacheRepository>();
        services.AddScoped<IMatchResultNoCacheRepository, MatchResultNoCacheRepository>();
    }
    

    public static void ConfigureMessageBroker(this IServiceCollection services)
    {
        services.AddSingleton<IEventProducer, KafkaEventProducer>();
        services.AddSingleton<IEventConsumer<SportValidationEvent>>(sp => 
            new KafkaEventConsumer<SportValidationEvent>(
                sp.GetRequiredService<IOptions<KafkaSettings>>(),
                sp.GetRequiredService<ILogger<KafkaEventConsumer<SportValidationEvent>>>()));

        services.AddHostedService<BetValidationConsumer>();
    }   
}