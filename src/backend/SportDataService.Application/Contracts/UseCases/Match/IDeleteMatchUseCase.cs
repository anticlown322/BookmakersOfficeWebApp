namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IDeleteMatchUseCase
{
    Task ExecuteAsync(string matchId, CancellationToken cancellationToken);
}