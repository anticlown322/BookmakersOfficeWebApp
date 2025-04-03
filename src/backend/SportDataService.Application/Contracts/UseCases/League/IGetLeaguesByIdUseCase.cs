using SportDataService.Application.DTO.League;

namespace SportDataService.Application.Contracts.UseCases.League;

public interface IGetLeagueByIdUseCase
{
    Task<LeagueGetDto> ExecuteAsync(string leagueId, CancellationToken cancellationToken);
}