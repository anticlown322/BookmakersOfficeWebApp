using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using UserService.GrpcService.Extensions;
using UserService.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(
            IPAddress.Any,
            50023,
            listenOptions =>
            {
                listenOptions.Protocols = HttpProtocols.Http2;

                // Для HTTP:
                // listenOptions.UseHttps(); // Закомментируйте для HTTP
            });
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