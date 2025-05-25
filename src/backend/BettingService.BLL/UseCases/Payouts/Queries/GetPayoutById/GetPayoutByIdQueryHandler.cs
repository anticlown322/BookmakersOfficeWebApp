using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Payouts.Queries.GetPayoutById;

public sealed class GetPayoutByIdQueryHandler(
    IPayoutRepository payoutRepository,
    ILogger<GetPayoutByIdQueryHandler> logger)
    : IRequestHandler<GetPayoutByIdQuery, Payout>
{
    public async Task<Payout> Handle(GetPayoutByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting payout by id {request.Id}...");

        var payout = await payoutRepository.GetByIdAsync(request.Id, cancellationToken);
        if (payout == null)
        {
            logger.LogWarning($"Payout with id {request.Id} not found");

            throw new PayoutNotFoundByIdException(request.Id);
        }

        logger.LogInformation($"Successfully retrieved payout with id {request.Id}");

        return payout;
    }
}