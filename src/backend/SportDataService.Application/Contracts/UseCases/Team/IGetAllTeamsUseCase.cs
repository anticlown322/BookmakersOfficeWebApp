using SportDataService.Application.DTO.Team;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.Team;

public interface IGetAllTeamsUseCase
{
    Task<(IEnumerable<TeamGetDto> teams, MetaData metaData)> ExecuteAsync(TeamParameters teamParameters, CancellationToken cancellationToken);
}