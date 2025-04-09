using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Services.Hangfire;

public class HangfireJobExecutor(
    IRefreshTournamentsUseCase refreshTournamentsUseCase)
    : IBackgroundJobExecutor
{
    public async Task ExecuteAsync(string jobId)
    {
        switch (jobId)
        {
            case "SportDataUpdate":
            {
                await refreshTournamentsUseCase.ExecuteAsync(default);
                break;
            }
        }
    }
}