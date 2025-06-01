using Microsoft.AspNetCore.SignalR;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.Contracts.Services.Signaling;

namespace SportDataService.Infrastructure.Services.SignalR.Implementations;

public class PrematchHub : Hub<IPrematchClient>
{
}