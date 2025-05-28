using Hangfire;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Settings;

namespace SportDataService.Infrastructure.Services.Hangfire;

public class HangfireJobScheduler(
    IRecurringJobManager jobManager,
    IOptions<HangfireSettings> settings,
    IServiceProvider services,
    ILogger<HangfireJobScheduler> logger)
    : IHostedService
{
    private readonly HangfireSettings _settings = settings.Value;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation($"Starting Hangfire JobScheduler...");

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

        logger.LogInformation($"Hangfire JobScheduler has started.");

        return Task.CompletedTask;
    }

    public async Task ExecuteJob(string jobId)
    {
        logger.LogInformation($"Executing Hangfire JobScheduler job with id {jobId}.");

        using var scope = services.CreateScope();
        var executor = scope.ServiceProvider.GetRequiredService<IBackgroundJobExecutor>();
        await executor.ExecuteAsync(jobId);

        logger.LogInformation($"Hangfire JobScheduler job has been executed.");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping Hangfire JobScheduler...");

        return Task.CompletedTask;
    }
}