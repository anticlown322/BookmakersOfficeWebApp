using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Application.Contracts.Services.Signaling;

public interface IPrematchNotificationService
{
    Task NotifyPrematchUpdatedAsync(IEnumerable<TournamentGetDto> tournaments, MetaData metaData);
}