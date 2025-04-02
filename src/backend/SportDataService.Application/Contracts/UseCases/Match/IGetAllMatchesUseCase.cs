using SportDataService.Application.DTO.Match;
using SportDataService.Application.UseCases.Match;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IGetAllMatchesUseCase
{
    Task<(IEnumerable<MatchGetDto> matches, MetaData metaData)> ExecuteAsync(MatchParameters matchParams, CancellationToken cancellationToken);
}