using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.Contracts.UseCases.Team;

public interface ICreateTeamUseCase
{
    Task<TeamGetDto> ExecuteAsync(TeamCreateDto teamCreateDto, CancellationToken cancellationToken);
}