using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserService.GrpcService.Extensions;
using UserService.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddDockerSecrets();

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Any, 50023, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
    });

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