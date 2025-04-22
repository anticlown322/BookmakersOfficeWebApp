using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Payments.Queries.GetPayoutById;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Payments.Queries.GetPayoutByBetId;

public sealed class GetPayoutByBetIdQueryHandler(
    IPayoutRepository payoutRepository,
    IBetRepository betRepository)
    : IRequestHandler<GetPayoutByBetIdQuery, Payout>
{
    public async Task<Payout> Handle(GetPayoutByBetIdQuery request, CancellationToken cancellationToken)
    {
        var bet = await betRepository.GetByIdAsync(request.BetId, cancellationToken);
        if (bet is null)
        {
            throw new BetNotFoundByIdException(request.BetId);
        }

        var payout = await payoutRepository.GetByBetIdAsync(request.BetId, cancellationToken);
        if (payout == null)
        {
            throw new PayoutNotFoundByBetIdException(request.BetId);
        }

        return payout;
    }
}