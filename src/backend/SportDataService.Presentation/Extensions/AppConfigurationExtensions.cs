using System.Text.Json;
using Hangfire;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Settings;
using SportDataService.Infrastructure.Services.Hangfire;
using SportDataService.Infrastructure.Services.SignalR.Implementations;

namespace SportDataService.Presentation.Extensions;

public static class AppConfigurationExtensions
{
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
                    DashboardTitle = "SportData Jobs",
                });
        }
    }

    public static void MapSingalR(this WebApplication app)
    {
        app.MapHub<PrematchHub>("/hubs/prematch");
        app.MapHub<ResultsHub>("/hubs/results");
    }
}