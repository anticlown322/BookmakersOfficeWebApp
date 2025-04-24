using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Queries.GetBetById;

public sealed class GetBetByIdQueryHandler(IBetRepository betRepository)
    : IRequestHandler<GetBetByIdQuery, Bet>
{
    public async Task<Bet> Handle(GetBetByIdQuery request, CancellationToken cancellationToken)
    {
        var bet = await betRepository.GetByIdAsync(request.id, cancellationToken);
        if (bet == null)
        {
            throw new BetNotFoundByIdException(request.id);
        }

        return bet;
    }
}