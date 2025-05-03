using BettingService.BLL.Contracts.Services;
using BettingService.BLL.UseCases.Bets.Commands.UpdateActiveBets;
using BettingService.BLL.UseCases.Bets.Commands.UpdatePendingBets;
using BettingService.BLL.UseCases.Payouts.Commands.ProcessPayouts;
using BettingService.DAL.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BettingService.BLL.Services.Hangfire;

public class HangfireJobExecutor(
    IDbContextFactory<RepositoryContext> dbFactory,
    IMediator mediator,
    ILoggerService logger)
    : IBackgroundJobExecutor
{
    public async Task ExecuteAsync(string jobId)
    {
        await using var db = await dbFactory.CreateDbContextAsync();

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