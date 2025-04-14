using System.Linq.Expressions;
using Hangfire;
using SportDataService.Application.Contracts.Services;

namespace SportDataService.Infrastructure.Services.Hangfire;

public class HangfireBackgroundJobService(
    IBackgroundJobClient backgroundJobClient,
    IRecurringJobManager recurringJobManager)
    : IBackgroundJobService
{
    public void ScheduleRecurringJob(string jobId, Expression<Action> jobMethod, string cronExpression)
    {
        recurringJobManager.AddOrUpdate(jobId, jobMethod, cronExpression);
    }

    public string EnqueueJob(Expression<Action> jobMethod)
    {
        return backgroundJobClient.Enqueue(jobMethod);
    }

    public void DeleteRecurringJob(string jobId)
    {
        recurringJobManager.RemoveIfExists(jobId);
    }
}