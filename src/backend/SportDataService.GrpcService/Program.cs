using System.Net;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using SportDataService.Domain.Models.Settings;
using SportDataService.GrpcService.Extensions;
using SportDataService.GrpcService.Services;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "Properties"))
        .AddJsonFile("secrets.json", optional: true, reloadOnChange: true);

    builder.Services.Configure<SportDataDbSettings>(
        builder.Configuration.GetSection("DatabaseSettings"));

    builder.WebHost.ConfigureKestrel(options =>
    {
        options.Listen(IPAddress.Any, 50022, listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
    });
    builder.Services.ConfigureGrpc();

    builder.Services.ConfigureMongoDbMappings();
    builder.Services.ConfigureMongoDbContext();
    builder.Services.AddSportDataDb();
    builder.Services.AddRepositories();
}

var app = builder.Build();
{
    app.MapGrpcService<SportDataGrpcService>();
}

app.Run();