using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Services.Hangfire;

public class HangfireJobExecutor(
    IRefreshTournamentsUseCase refreshTournamentsUseCase,
    IRefreshTournamentResultsUseCase refreshTournamentResultsUseCase)
    : IBackgroundJobExecutor
{
    public async Task ExecuteAsync(string jobId)
    {
        switch (jobId)
        {
            case HangfireJobNames.UpdatePrematch:
            {
                await refreshTournamentsUseCase.ExecuteAsync(default);
                break;
            }

            case HangfireJobNames.UpdateResults:
            {
                await refreshTournamentResultsUseCase.ExecuteAsync(default);
                break;
            }
        }
    }
}