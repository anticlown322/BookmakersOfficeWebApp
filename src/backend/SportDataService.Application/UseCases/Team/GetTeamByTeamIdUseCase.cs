using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Common;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Team;

public class GetTeamByTeamIdUseCase(
    ITeamRepository teamRepository,
    IMapper mapper,
    ILogger<GetTeamByTeamIdUseCase> logger)
    : IGetTeamByTeamIdUseCase
{
    public async Task<TeamGetDto> ExecuteAsync(string teamId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Attempting to get team with team id {teamId}");

        cancellationToken.ThrowIfCancellationRequested();

        var team = await teamRepository.GetTeamByTeamIdAsync(teamId, cancellationToken);
        if (team == null)
        {
            logger.LogWarning($"Team with team id {teamId} not found");

            throw new TeamNotFoundByTeamIdException(teamId);
        }

        var result = mapper.Map<TeamGetDto>(team);

        logger.LogInformation($"Successfully retrieved team with team id {teamId}");

        return result;
    }
}