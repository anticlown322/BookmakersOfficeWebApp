using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Common;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Team;

public sealed class GetTeamByIdUseCase(
    ITeamRepository teamRepository,
    IMapper mapper,
    ILogger<GetTeamByIdUseCase> logger)
    : IGetTeamByIdUseCase
{
    public async Task<TeamGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting team with id {id}");

        if (!ObjectId.TryParse(id, out _))
        {
            logger.LogWarning($"Invalid id {id}");

            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var team = await teamRepository.GetByIdAsync(id, cancellationToken);
        if (team == null)
        {
            logger.LogWarning($"Team with id {id} not found");

            throw new TeamNotFoundByIdException(id);
        }

        var result = mapper.Map<TeamGetDto>(team);

        logger.LogInformation($"Successfully retrieved team with id {id}");

        return result;
    }
}