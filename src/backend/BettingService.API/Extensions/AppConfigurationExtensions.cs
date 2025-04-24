using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Services.Hangfire;
using BettingService.DAL.Models.Settings;
using Hangfire;
using Microsoft.Extensions.Options;

namespace BettingService.API.Extensions;

public static class AppConfigurationExtensions
{
    public static async Task ApplyMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var migrator = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
        await migrator.MigrateAsync();
    }

    public static void ConfigureHangfireDashboard(this WebApplication app)
    {
        var hangfireSettings = app.Services.GetRequiredService<IOptions<HangfireSettings>>().Value;

        if (hangfireSettings.EnableDashboard)
        {
            app.UseHangfireDashboard(
                hangfireSettings.DashboardPath,
                new DashboardOptions
                {
                    Authorization = new[] { new HangfireDashboardAuthorizationFilter() },
                    IsReadOnlyFunc = _ => false,
                    StatsPollingInterval = 2000,
                    DashboardTitle = "Betting service Jobs",
                });
        }
    }
}