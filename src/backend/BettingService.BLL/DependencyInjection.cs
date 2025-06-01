using System.Reflection;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.DTO.MappingProfiles;
using BettingService.BLL.Services;
using BettingService.BLL.Services.Hangfire;
using BettingService.BLL.Services.Kafka;
using BettingService.BLL.Validation;
using BettingService.BLL.Validation.Validators;
using BettingService.DAL.Models.Settings;
using BettingService.DAL.Models.Settings.Kafka;
using FluentValidation;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Formatting.Json;

namespace BettingService.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .WriteTo.Console()
            .WriteTo.Http(
                requestUri: configuration["Logstash:Uri"],
                queueLimitBytes: null,
                textFormatter: new JsonFormatter())
            .CreateLogger();

        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddAutoMapper(
            cfg =>
            {
                cfg.AddProfile<GetBetMappingProfile>();
                cfg.AddProfile<GetPayoutMappingProfile>();
            },
            AppDomain.CurrentDomain.GetAssemblies());

        services.AddValidatorsFromAssembly(typeof(PlaceBetCommandValidator).Assembly);

        services.Configure<HangfireSettings>(configuration.GetSection("HangfireSettings"));
        services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();
        services.AddSingleton<IBackgroundJobExecutor, HangfireJobExecutor>();

        var connectionString = configuration.GetConnectionString("HangfireDbConnection");
        var hangfireSettings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>()!;
        services.AddHangfire(config => config
            .UseMongoStorage(
                connectionString,
                hangfireSettings.DatabaseName,
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
            .UseSimpleAssemblyNameTypeSerializer()
            .UseRecommendedSerializerSettings()
            .UseFilter(new AutomaticRetryAttribute { Attempts = 3 }));

        services.AddHangfireServer(options =>
        {
            options.ServerName = "Betting.Background";
            options.Queues = new[] { "default", "critical" };
            options.WorkerCount = 1;
        });

        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
        services.AddSingleton<IKafkaConsumerService, KafkaConsumerService>();
        
        return services;
    }
}