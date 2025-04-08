using SportDataService.Application.Contracts.Services;
using SportDataService.Infrastructure.Configs;
using SportDataService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = new ConfigurationBuilder()
        .AddSecretsYaml()
        .Build();

    builder.Configuration.AddConfiguration(configuration);
    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.ConfigureMongoDbMappings();
    builder.Services.ConfigureMongoDbContext();
    builder.Services.AddHostedService<MongoDbInitializer>();
    builder.Services.AddRepositories();

    builder.Services.ConfigureNLog();
    builder.Services.ConfigureLoggerService();

    builder.Services.ConfigureDataCollectionService();
    builder.Services.ConfigureUseCases();
    builder.Services.ConfigureAutoMapper();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    var logger = app.Services.GetService<ILoggerService>();
    app.ConfigureExceptionHandler(logger);

    app.UseHttpsRedirection();
    app.MapControllers();
}

app.Run();