using System.Reflection;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.DTO.Bet;
using BettingService.BLL.DTO.MappingProfiles;
using BettingService.BLL.DTO.Payout;
using BettingService.BLL.Services;
using BettingService.BLL.Services.Hangfire;
using BettingService.BLL.Validation.Validators;
using BettingService.DAL.Models.Settings;
using FluentValidation;
using Hangfire;
using Hangfire.Mongo;
using Hangfire.Mongo.Migration.Strategies;
using Hangfire.Mongo.Migration.Strategies.Backup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BettingService.BLL;

public static class DependencyInjection
{
    public static IServiceCollection AddBusinessLogicLayer(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<ILoggerService, LoggerService>();
        services.AddScoped<IDatabaseMigrationService, DatabaseMigrationService>();

        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        services.AddAutoMapper(
            cfg =>
            {
                cfg.AddProfile<GetBetMappingProfile>();
                cfg.AddProfile<GetPayoutMappingProfile>();
            },
            AppDomain.CurrentDomain.GetAssemblies());

        services.AddTransient<IValidator<CreatePayoutDto>, CreatePayoutDtoValidator>();
        services.AddTransient<IValidator<PlaceBetDto>, PlaceBetDtoValidator>();

        services.AddSingleton<IBackgroundJobService, HangfireBackgroundJobService>();
        services.AddScoped<IBackgroundJobExecutor, HangfireJobExecutor>();

        var settings = configuration.GetSection("HangfireSettings").Get<HangfireSettings>()!;
        services.AddHangfire(config => config
            .UseMongoStorage(
                settings.ConnectionString,
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
            options.ServerName = "Betting.Background";
            options.Queues = new[] { "default", "critical" };
            options.WorkerCount = Environment.ProcessorCount * 2;
        });

        return services;
    }
}