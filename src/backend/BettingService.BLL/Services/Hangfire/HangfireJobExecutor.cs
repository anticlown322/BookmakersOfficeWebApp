using BettingService.BLL.Contracts.Services;

namespace BettingService.BLL.Services.Hangfire;

public class HangfireJobExecutor()
    : IBackgroundJobExecutor
{
    public async Task ExecuteAsync(string jobId)
    {
        switch (jobId)
        {
            case "UpdateBets":
            {
                // TODO: add bets updating job when grpc interaction is implemented
                break;
            }

            case "NotifyUsers":
            {
                // TODO: add user notifying job when grpc interaction is implemented
                break;
            }
        }
    }
}