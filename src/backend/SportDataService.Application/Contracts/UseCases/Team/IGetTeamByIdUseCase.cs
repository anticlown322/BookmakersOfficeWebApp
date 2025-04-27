using SportDataService.Application.DTO.Common;

namespace SportDataService.Application.Contracts.UseCases.Team;

public interface IGetTeamByIdUseCase
{
    Task<TeamGetDto> ExecuteAsync(string id, CancellationToken cancellationToken);
}