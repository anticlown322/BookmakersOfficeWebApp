using UserService.Application.Contracts.Services;
using UserService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    builder.Configuration
        .AddEnvironmentVariables()
        .AddJsonFile("appsettings.json", optional: true)
        .AddJsonFile("/app/Properties/secrets.json", optional: true)
        .AddDockerSecrets();

    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.ConfigureNLog();
    builder.Services.ConfigureLoggerService();

    builder.Services.AddDatabaseMigrationService();
    builder.Services.ConfigureSqlContext(builder.Configuration);
    builder.Services.AddUserRepository();

    builder.Services.AddAuthentication();
    builder.Services.ConfigureIdentity();
    builder.Services.ConfigureJwt(builder.Configuration);
    builder.Services.AddTokenService();
    builder.Services.AddAuthorizationPolicies();

    builder.Services.ConfigureUseCases();
    builder.Services.AddEmailService(builder.Configuration);
    builder.Services.AddValidators();
    builder.Services.ConfigureAutoMapper();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    var logger = app.Services.GetRequiredService<ILoggerService>();
    app.ConfigureExceptionHandler(logger);

    await app.ApplyMigrations();

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();
    app.MapControllers();
}

app.Run();