using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserService.GrpcService.Extensions;
using UserService.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("/app/Properties/secrets.json", optional: true)
        .AddDockerSecrets();

    builder.Services.ConfigureDbContext(builder.Configuration);
    builder.Services.ConfigureGrpc();
    builder.Services.ConfigureIdentity();
    builder.Services.AddRepository();
}

var app = builder.Build();
{
    app.MapGrpcService<UserGrpcServiceImplementation>();
}

app.Run();