using BettingService.BLL.Contracts.Services;
using BettingService.BLL.UseCases.Bets.Commands.UpdateActiveBets;
using BettingService.BLL.UseCases.Bets.Commands.UpdatePendingBets;
using BettingService.BLL.UseCases.Payouts.Commands.ProcessPayouts;
using MediatR;

namespace BettingService.BLL.Services.Hangfire;

public class HangfireJobExecutor(
    IMediator mediator)
    : IBackgroundJobExecutor
{
    public async Task ExecuteAsync(string jobId)
    {
        switch (jobId)
        {
            case "UpdatePendingBets":
            {
                await mediator.Send(new UpdatePendingBetsCommand(), CancellationToken.None);
                break;
            }

            case "UpdateBetsResults":
            {
                await mediator.Send(new UpdateActiveBetsCommand(), CancellationToken.None);
                break;
            }

            case "ProcessPayouts":
            {
                await mediator.Send(new ProcessPayoutsCommand(), CancellationToken.None);
                break;
            }
        }
    }
}