using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.Contracts.UseCases.Team;

public interface IUpdateTeamUseCase
{
    Task ExecuteAsync(string teamId, TeamUpdateDto teamUpdateDto, CancellationToken cancellationToken);
}