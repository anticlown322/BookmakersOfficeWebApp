using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Common;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Team;

public class GetTeamByTeamIdUseCase(
    ITeamRepository teamRepository,
    IMapper mapper)
    : IGetTeamByTeamIdUseCase
{
    public async Task<TeamGetDto> ExecuteAsync(string teamId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var team = await teamRepository.GetTeamByTeamIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            throw new TeamNotFoundByTeamIdException(teamId);
        }

        return mapper.Map<TeamGetDto>(team);
    }
}