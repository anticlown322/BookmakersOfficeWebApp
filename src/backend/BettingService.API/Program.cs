using BettingService.API.Extensions;
using BettingService.API.Middlewares;
using BettingService.BLL;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Services.Hangfire;
using BettingService.DAL;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: false);

    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.ConfigureNLog();

    builder.Services.AddDataAccessLayer(builder.Configuration);
    builder.Services.AddBusinessLogicLayer(builder.Configuration );

    builder.Services.ConfigureAuth(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextAccessor();

    builder.Services.AddGrpcClients(builder.Configuration);

    builder.Services.AddHostedService<HangfireJobScheduler>();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    var logger = app.Services.GetService<ILoggerService>();
    app.ConfigureExceptionHandler(logger);

    await app.ApplyMigrations();

    app.UseAuthentication();
    app.UseAuthorization();

    app.ConfigureHangfireDashboard();

    app.MapControllers();
}

app.Run();