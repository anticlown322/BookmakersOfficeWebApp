namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IRefreshTournaments
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}