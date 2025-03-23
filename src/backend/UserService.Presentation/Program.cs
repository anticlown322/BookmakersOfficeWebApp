using NLog;
using UserService.Application.Contracts;
using UserService.Presentation.Extensions;

var builder = WebApplication.CreateBuilder(args);
{
    var configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Properties\nlog.config");
    LogManager.Setup().LoadConfigurationFromFile(configFilePath);
    builder.Services.ConfigureLoggerService();

    builder.Services.ConfigureRepositoryManager();
    builder.Services.ConfigureSqlContext(builder.Configuration);
    builder.Services.ConfigureAutoMapper();

    builder.Services.AddAuthentication();
    builder.Services.ConfigureIdentity();
    builder.Services.ConfigureJwt(builder.Configuration);
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
