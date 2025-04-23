using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Models.Settings;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Hangfire;

public class HangfireJobScheduler(
    IRecurringJobManager jobManager,
    IOptions<HangfireSettings> settings,
    IServiceProvider services)
    : IHostedService
{
    private readonly HangfireSettings _settings = settings.Value;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (_settings.RecurringJobs == null)
        {
            return Task.CompletedTask;
        }

        foreach (var job in _settings.RecurringJobs)
        {
            jobManager.AddOrUpdate(
                job.Key,
                () => ExecuteJob(job.Key),
                job.Value);
        }

        return Task.CompletedTask;
    }

    public async Task ExecuteJob(string jobId)
    {
        using var scope = services.CreateScope();
        var executor = scope.ServiceProvider.GetRequiredService<IBackgroundJobExecutor>();
        await executor.ExecuteAsync(jobId);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}