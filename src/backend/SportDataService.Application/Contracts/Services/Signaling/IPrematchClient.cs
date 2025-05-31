using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Application.Contracts.Services.Signaling;

public interface IPrematchClient
{
    Task PrematchUpdated(IEnumerable<TournamentGetDto> tournaments, MetaData metaData);
}