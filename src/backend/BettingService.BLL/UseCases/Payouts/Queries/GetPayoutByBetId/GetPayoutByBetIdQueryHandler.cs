using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Payouts.Queries.GetPayoutByBetId;

public sealed class GetPayoutByBetIdQueryHandler(
    IPayoutRepository payoutRepository,
    IBetRepository betRepository,
    ILogger<GetPayoutByBetIdQueryHandler> logger)
    : IRequestHandler<GetPayoutByBetIdQuery, Payout>
{
    public async Task<Payout> Handle(GetPayoutByBetIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting payout by bet id {request.BetId}");

        var bet = await betRepository.GetByIdAsync(request.BetId, cancellationToken);
        if (bet is null)
        {
            logger.LogWarning($"Unable to find bet with id {request.BetId}");

            throw new BetNotFoundByIdException(request.BetId);
        }

        var payout = await payoutRepository.GetByBetIdAsync(request.BetId, cancellationToken);
        if (payout == null)
        {
            logger.LogWarning($"Unable to find payout with bet id {request.BetId}");

            throw new PayoutNotFoundByBetIdException(request.BetId);
        }

        logger.LogInformation($"Successfully retrieved payout by bet id {request.BetId}");

        return payout;
    }
}