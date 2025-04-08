using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.Contracts.UseCases.Team;

public interface IGetTeamByTeamIdUseCase
{
    Task<TeamGetDto> ExecuteAsync(string teamId, CancellationToken cancellationToken);
}