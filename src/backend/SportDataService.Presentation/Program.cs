using Hangfire;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Settings;
using SportDataService.Infrastructure.Configs;
using SportDataService.Infrastructure.Services.Hangfire;
using SportDataService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("/app/Properties/secrets.json", optional: true)
        .AddDockerSecrets();

    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.ConfigureAuth(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextAccessor();

    builder.Services.ConfigureMongoDbMappings();
    builder.Services.ConfigureMongoDbContext(builder.Configuration);
    builder.Services.AddSportDataDb();
    builder.Services.AddHostedService<MongoDbInitializer>();

    builder.Services.ConfigureRedis(builder.Configuration);
    builder.Services.ConfigureCacheService();

    builder.Services.AddRepositories();

    builder.Services.ConfigureNLog();
    builder.Services.ConfigureLoggerService();

    builder.Services.ConfigureDataCollectionService();
    builder.Services.ConfigureUseCases();
    builder.Services.ConfigureAutoMapper();

    builder.Services.ConfigureBackgroundJobService();
    builder.Services.ConfigureHangfire(builder.Configuration);
    builder.Services.ConfigureBackgroundJobExecutor();
    builder.Services.AddHostedService<HangfireJobScheduler>();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    var logger = app.Services.GetService<ILoggerService>();
    app.ConfigureExceptionHandler(logger);

    app.UseAuthentication();
    app.UseAuthorization();

    app.ConfigureHangfireDashboard();

    app.MapControllers();
}

app.Run();