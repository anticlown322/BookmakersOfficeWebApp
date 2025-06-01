using Microsoft.AspNetCore.SignalR;
using SportDataService.Application.Contracts.Services.Signaling;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Services.SignalR.Implementations;

public class ResultsNotificationService : IResultsNotificationService
{
    private readonly IHubContext<ResultsHub, IResultsClient> _hubContext;

    public ResultsNotificationService(IHubContext<ResultsHub, IResultsClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyResultsUpdatedAsync(IEnumerable<TournamentResultGetDto> results, MetaData metaData)
    {
        await _hubContext.Clients.All.ResultsUpdated(results, metaData);
    }
}