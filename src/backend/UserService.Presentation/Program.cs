using UserService.Application.Contracts;
using UserService.Application.Contracts.Services;
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

    builder.Services.ConfigureSqlContext();
    builder.Services.AddUserRepository();

    builder.Services.AddAuthentication();
    builder.Services.ConfigureIdentity();
    builder.Services.ConfigureJwt();
    builder.Services.AddTokenService();
    builder.Services.AddAuthorizationPolicies();

    builder.Services.ConfigureUseCases();
    builder.Services.AddEmailService();
    builder.Services.AddValidators();
    builder.Services.ConfigureAutoMapper();

    builder.Services.AddControllers();
    builder.Services.ConfigureApiBehaviorOptions();
}

var app = builder.Build();
{
    var logger = app.Services.GetRequiredService<ILoggerService>();
    app.ConfigureExceptionHandler(logger);

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseHttpsRedirection();
    app.MapControllers();
}

app.Run();
