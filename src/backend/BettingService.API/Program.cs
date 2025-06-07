using BettingService.API.Extensions;
using BettingService.API.Middlewares;
using BettingService.BLL;
using BettingService.BLL.Services.Hangfire;
using BettingService.DAL;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("/app/Properties/secrets.json", optional: true)
        .AddDockerSecrets();

    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.AddDataAccessLayer(builder.Configuration);
    builder.Services.AddBusinessLogicLayer(builder.Configuration);
    builder.Host.UseSerilog();

    builder.Services.ConfigureAuth(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddGrpcClients(builder.Configuration);

    builder.Services.AddHostedService<HangfireJobScheduler>();

    builder.Services.ConfigureCors();
    
    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    app.ConfigureExceptionHandler();

    await app.ApplyMigrations();

    app.UseAuthentication();
    app.UseAuthorization();

    app.ConfigureHangfireDashboard();

    app.UseCors();
    
    app.MapControllers();
}

app.Run();