using SportDataService.Application.DTO.League;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.League;

public interface IGetAllLeaguesUseCase
{
    Task<(IEnumerable<LeagueGetDto> leagues, MetaData metaData)> ExecuteAsync(LeagueParameters leagueParameters, CancellationToken cancellationToken);
}