using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Odds;
using SportDataService.Application.DTO.Odds;
using SportDataService.Application.DTO.Odds;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Odds;

public sealed class GetAllOddsUseCase(
    IOddsRepository oddsRepository,
    IMapper mapper)
    : IGetAllOddsUseCase
{
    public async Task<(IEnumerable<OddsGetDto> odds, MetaData metaData)> ExecuteAsync(OddsParameters oddsParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var oddsWithMetaData = await oddsRepository.FindAllOddsAsync(oddsParameters, cancellationToken);

        var oddsGetDtos = mapper.Map<IEnumerable<OddsGetDto>>(oddsWithMetaData);

        return (
            odds: oddsGetDtos,
            metaData: oddsWithMetaData.MetaData);
    }
}