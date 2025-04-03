namespace SportDataService.Application.Contracts.UseCases.League;

public interface IDeleteLeagueUseCase
{
    Task ExecuteAsync(string leagueId, CancellationToken cancellationToken);
}