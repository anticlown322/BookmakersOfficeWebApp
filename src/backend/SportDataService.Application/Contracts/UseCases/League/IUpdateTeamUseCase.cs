using SportDataService.Application.DTO.League;

namespace SportDataService.Application.Contracts.UseCases.League;

public interface IUpdateLeagueUseCase
{
    Task ExecuteAsync(string leagueId, LeagueUpdateDto leagueUpdateDto, CancellationToken cancellationToken);
}