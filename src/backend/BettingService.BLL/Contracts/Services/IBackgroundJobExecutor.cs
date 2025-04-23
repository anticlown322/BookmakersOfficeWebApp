namespace BettingService.BLL.Contracts.Services;

public interface IBackgroundJobExecutor
{
    Task ExecuteAsync(string jobId);
}