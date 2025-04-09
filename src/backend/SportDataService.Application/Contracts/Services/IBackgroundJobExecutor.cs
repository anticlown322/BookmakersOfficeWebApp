namespace SportDataService.Application.Contracts.Services;

public interface IBackgroundJobExecutor
{
    Task ExecuteAsync(string jobId);
}