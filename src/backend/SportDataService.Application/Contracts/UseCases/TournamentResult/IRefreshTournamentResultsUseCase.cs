namespace SportDataService.Application.Contracts.UseCases.TournamentResult;

public interface IRefreshTournamentResultsUseCase
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}