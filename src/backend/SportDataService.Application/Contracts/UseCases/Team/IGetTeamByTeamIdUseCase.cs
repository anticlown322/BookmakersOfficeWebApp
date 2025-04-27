using SportDataService.Application.DTO.Common;

namespace SportDataService.Application.Contracts.UseCases.Team;

public interface IGetTeamByTeamIdUseCase
{
    Task<TeamGetDto> ExecuteAsync(string teamId, CancellationToken cancellationToken);
}