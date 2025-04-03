using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Team;
using SportDataService.Application.DTO.Team;
using SportDataService.Application.DTO.Team;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Team;

public sealed class GetAllTeamsUseCase(
    ITeamRepository teamRepository,
    IMapper mapper)
    : IGetAllTeamsUseCase
{
    public async Task<(IEnumerable<TeamGetDto> teams, MetaData metaData)> ExecuteAsync(TeamParameters teamParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var teamsWithMetaData = await teamRepository.FindAllTeamsAsync(teamParameters, cancellationToken);

        var teamGetDtos = mapper.Map<IEnumerable<TeamGetDto>>(teamsWithMetaData);

        return (
            teams: teamGetDtos,
            metaData: teamsWithMetaData.MetaData);
    }
}