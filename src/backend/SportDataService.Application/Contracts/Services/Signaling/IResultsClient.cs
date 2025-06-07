using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Application.Contracts.Services.Signaling;

public interface IResultsClient
{
    Task ResultsUpdated(IEnumerable<TournamentResultGetDto> results, MetaData metaData);
}