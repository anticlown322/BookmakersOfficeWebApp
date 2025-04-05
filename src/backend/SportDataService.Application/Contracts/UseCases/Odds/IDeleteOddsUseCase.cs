namespace SportDataService.Application.Contracts.UseCases.Odds;

public interface IDeleteOddsUseCase
{
    Task ExecuteAsync(string oddsId, CancellationToken cancellationToken);
}