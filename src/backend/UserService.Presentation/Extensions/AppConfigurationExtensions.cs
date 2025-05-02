using UserService.Application.Contracts.Services;

namespace UserService.Presentation.Extensions;

public static class AppConfigurationExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
        await migrator.MigrateAsync();
    }
}