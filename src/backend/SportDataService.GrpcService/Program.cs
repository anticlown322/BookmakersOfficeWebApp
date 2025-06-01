using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SportDataService.GrpcService.Extensions;
using SportDataService.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("/app/Properties/secrets.json", optional: true)
        .AddDockerSecrets();

    builder.Services.AddLogging(loggingBuilder => {
        loggingBuilder.AddConsole();
        loggingBuilder.AddDebug();
    });
    
    builder.Services.AddAppSettings(builder.Configuration);
    
    builder.Services.ConfigureGrpc();
    builder.Services.ConfigureMongoDbMappings();
    builder.Services.ConfigureMongoDbContext(builder.Configuration);
    builder.Services.AddSportDataDb(builder.Configuration);
    builder.Services.AddRepositories();
    builder.Services.ConfigureMessageBroker();
}

var app = builder.Build();
{
    app.MapGrpcService<SportDataGrpcService>();
}

app.Run();