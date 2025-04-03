namespace SportDataService.Application.Contracts.UseCases.Team;

public interface IDeleteTeamUseCase
{
    Task ExecuteAsync(string teamId, CancellationToken cancellationToken);
}