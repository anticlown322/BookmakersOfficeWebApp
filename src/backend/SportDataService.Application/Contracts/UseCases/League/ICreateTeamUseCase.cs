using SportDataService.Application.DTO.League;

namespace SportDataService.Application.Contracts.UseCases.League;

public interface ICreateLeagueUseCase
{
    Task<LeagueGetDto> ExecuteAsync(LeagueCreateDto leagueCreateDto, CancellationToken cancellationToken);
}