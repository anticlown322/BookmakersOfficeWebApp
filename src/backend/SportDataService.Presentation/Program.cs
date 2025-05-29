using Hangfire;
using Microsoft.Extensions.Options;
using Serilog;
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

    builder.Services.ConfigureLogging(builder.Configuration);
    builder.Host.UseSerilog();

    builder.Services.ConfigureAuth(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextAccessor();

    builder.Services.ConfigureMongoDbMappings();
    builder.Services.ConfigureMongoDbContext(builder.Configuration);
    builder.Services.AddSportDataDb();
    builder.Services.AddHostedService<MongoDbInitializer>();
    builder.Services.AddRepositories();

    builder.Services.ConfigureDataCollectionService();
    builder.Services.ConfigureUseCases();
    builder.Services.ConfigureAutoMapper();

    builder.Services.ConfigureBackgroundJobService();
    builder.Services.ConfigureHangfire(builder.Configuration);
    builder.Services.ConfigureBackgroundJobExecutor();
    builder.Services.AddHostedService<HangfireJobScheduler>();

    builder.Services.ConfigureCors();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    app.ConfigureExceptionHandler();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseCors();
    app.ConfigureHangfireDashboard();

    app.MapControllers();
}

app.Run();