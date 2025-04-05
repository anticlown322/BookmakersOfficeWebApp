using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.Odds;

public interface IGetAllOddsUseCase
{
    Task<(IEnumerable<OddsGetDto> odds, MetaData metaData)> ExecuteAsync(OddsParameters oddsParameters, CancellationToken cancellationToken);
}