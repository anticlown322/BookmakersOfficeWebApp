using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Common;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Team;

public sealed class GetAllTeamsUseCase(
    ITeamRepository teamRepository,
    IMapper mapper,
    ILogger<GetAllTeamsUseCase> logger)
    : IGetAllTeamsUseCase
{
    public async Task<(IEnumerable<TeamGetDto> teams, MetaData metaData)> ExecuteAsync(
        TeamParameters teamParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting teams...");

        cancellationToken.ThrowIfCancellationRequested();

        var teamsWithMetaData = await teamRepository.FindAllTeamsAsync(teamParameters, cancellationToken);

        var teamGetDtos = mapper.Map<IEnumerable<TeamGetDto>>(teamsWithMetaData);

        logger.LogInformation($"Teams successfully retrieved");

        return (
            teams: teamGetDtos,
            metaData: teamsWithMetaData.MetaData);
    }
}