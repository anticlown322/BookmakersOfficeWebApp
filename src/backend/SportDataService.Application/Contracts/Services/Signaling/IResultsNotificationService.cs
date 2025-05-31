using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Application.Contracts.Services.Signaling;

public interface IResultsNotificationService
{
    Task NotifyResultsUpdatedAsync(IEnumerable<TournamentResultGetDto> results, MetaData metaData);
}