namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IRefreshTournamentsUseCase
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}