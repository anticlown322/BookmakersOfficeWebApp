using System.Linq.Expressions;

namespace SportDataService.Application.Contracts.Services;

public interface IBackgroundJobService
{
    void ScheduleRecurringJob(string jobId, Expression<Action> jobMethod, string cronExpression);
    string EnqueueJob(Expression<Action> jobMethod);
    void DeleteRecurringJob(string jobId);
}