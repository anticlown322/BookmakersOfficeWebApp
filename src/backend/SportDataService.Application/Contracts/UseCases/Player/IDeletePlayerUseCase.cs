namespace SportDataService.Application.Contracts.UseCases.Player;

public interface IDeletePlayerUseCase
{
    Task ExecuteAsync(string playerId, CancellationToken cancellationToken);
}