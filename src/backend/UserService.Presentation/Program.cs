using NLog;
using UserService.Application.Contracts;
using UserService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    var configuration = new ConfigurationBuilder()
        .AddSecretsYaml()
        .Build();

    builder.Configuration.AddConfiguration(configuration);
    builder.Services.AddAppSettings(builder.Configuration);

    builder.Services.ConfigureNLog();
    builder.Services.ConfigureLoggerService();

    builder.Services.ConfigureRepositoryManager();
    builder.Services.ConfigureSqlContext();
    builder.Services.ConfigureAutoMapper();

    builder.Services.AddAuthentication();
    builder.Services.ConfigureIdentity();
    builder.Services.ConfigureJwt();
    builder.Services.ConfigureAuthenticationManager();
    builder.Services.ConfigureUseCases();
    builder.Services.AddValidators();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    var logger = app.Services.GetRequiredService<ILoggerManager>();
    app.ConfigureExceptionHandler(logger);

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();
    app.MapControllers();
}

app.Run();
