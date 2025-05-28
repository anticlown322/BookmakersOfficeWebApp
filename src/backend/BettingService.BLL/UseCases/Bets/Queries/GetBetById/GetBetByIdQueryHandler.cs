using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.UseCases.Bets.Queries.GetBetById;

public sealed class GetBetByIdQueryHandler(
    IBetRepository betRepository,
    ILogger<GetBetByIdQueryHandler> logger)
    : IRequestHandler<GetBetByIdQuery, Bet>
{
    public async Task<Bet> Handle(GetBetByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting bet by id {request.id}");

        var bet = await betRepository.GetByIdAsync(request.id, cancellationToken);
        if (bet == null)
        {
            logger.LogWarning($"No bet with id {request.id} was found");

            throw new BetNotFoundByIdException(request.id);
        }

        logger.LogInformation($"Successfully retrieved bet with id {request.id}");

        return bet;
    }
}