namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IForceTournamentRefresh
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}