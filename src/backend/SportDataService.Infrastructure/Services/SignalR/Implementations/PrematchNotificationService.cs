using Microsoft.AspNetCore.SignalR;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.Services.Signaling;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Services.SignalR.Implementations;

public class PrematchNotificationService : IPrematchNotificationService
{
    private readonly IHubContext<PrematchHub, IPrematchClient> _hubContext;

    public PrematchNotificationService(IHubContext<PrematchHub, IPrematchClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyPrematchUpdatedAsync(IEnumerable<TournamentGetDto> tournaments, MetaData metaData)
    {
        await _hubContext.Clients.All.PrematchUpdated(tournaments, metaData);
    }
}