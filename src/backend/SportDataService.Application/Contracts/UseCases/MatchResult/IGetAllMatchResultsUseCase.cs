using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.MatchResult;

public interface IGetAllMatchResultsUseCase
{
    Task<(IEnumerable<MatchResultGetDto> matchResults, MetaData metaData)> ExecuteAsync(MatchResultParameters matchResultParameters, CancellationToken cancellationToken);
}