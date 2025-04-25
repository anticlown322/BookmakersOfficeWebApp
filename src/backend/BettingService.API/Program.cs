using BettingService.API.Extensions;
using BettingService.API.Middlewares;
using BettingService.BLL;
using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Services.Hangfire;
using BettingService.BLL.UseCases.Bets.Commands.PlaceBet;
using BettingService.DAL;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = new ConfigurationBuilder()
        .AddSecretsYaml()
        .Build();

    builder.Configuration.AddConfiguration(configuration);
    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.ConfigureNLog();

    builder.Services.AddDataAccessLayer(configuration);
    builder.Services.AddBusinessLogicLayer(configuration);

    builder.Services.ConfigureAuth(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextAccessor();

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