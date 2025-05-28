using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OcelotApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("/app/Properties/secrets.json", optional: true)
        .AddJsonFile("ocelot.json", optional: false)
        .AddDockerSecrets();
    
    builder.Services.ConfigureCors();
    builder.Services.ConfigureAuth(builder.Configuration);
    
    builder.Services.AddOcelot(builder.Configuration);
}

var app = builder.Build();
{
    app.UseCors("AllowAll");

    app.UseAuthentication();
    app.UseAuthorization();
    
    await app.UseOcelot();
}

app.Run();
