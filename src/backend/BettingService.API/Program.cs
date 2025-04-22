using BettingService.API.Extensions;
using BettingService.API.Middlewares;
using BettingService.BLL;
using BettingService.BLL.Contracts.Services;
using BettingService.DAL;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = new ConfigurationBuilder()
        .AddSecretsYaml()
        .Build();

    builder.Configuration.AddConfiguration(configuration);
    builder.Services.AddAppSettings(builder.Configuration);
    
    builder.Services.ConfigureNLog();

    builder.Services.AddDataAccessLayer(configuration);
    builder.Services.AddBusinessLogicLayer();
    
    builder.Services.ConfigureAuth(builder.Configuration);
    builder.Services.AddAuthorizationPolicies();
    builder.Services.AddHttpContextAccessor();
    
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
    app.MapControllers();
}

app.Run();