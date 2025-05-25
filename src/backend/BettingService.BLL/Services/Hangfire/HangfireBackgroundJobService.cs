using System.Linq.Expressions;
using BettingService.BLL.Contracts.Services;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.Services.Hangfire;

public class HangfireBackgroundJobService(
    IBackgroundJobClient backgroundJobClient,
    IRecurringJobManager recurringJobManager,
    ILogger<HangfireBackgroundJobService> logger)
    : IBackgroundJobService
{
    public void ScheduleRecurringJob(string jobId, Expression<Action> jobMethod, string cronExpression)
    {
        logger.LogInformation($"Scheduling recurring job with id {jobId}...");

        recurringJobManager.AddOrUpdate(jobId, jobMethod, cronExpression);

        logger.LogInformation($"Recurring job with id {jobId} has been scheduled.");
    }

    public string EnqueueJob(Expression<Action> jobMethod)
    {
        logger.LogInformation("Enqueueing job...");

        return backgroundJobClient.Enqueue(jobMethod);
    }

    public void DeleteRecurringJob(string jobId)
    {
        logger.LogInformation($"Deleting recurring job with id {jobId}...");

        recurringJobManager.RemoveIfExists(jobId);

        logger.LogInformation($"Recurring job with id {jobId} has been deleted.");
    }
}