using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Queries.GetPayoutById;

public sealed class GetPayoutByIdQueryHandler(IPayoutRepository payoutRepository)
    : IRequestHandler<GetPayoutByIdQuery, Payout>
{
    public async Task<Payout> Handle(GetPayoutByIdQuery request, CancellationToken cancellationToken)
    {
        var payout = await payoutRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payout == null)
        {
            throw new PayoutNotFoundByIdException(request.Id);
        }

        return payout;
    }
}