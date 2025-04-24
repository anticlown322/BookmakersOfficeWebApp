using BettingService.BLL.DTO.Bet;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Queries.GetAllBets;

public sealed record GetAllBetsQuery(BetParameters Parameters)
    : IRequest<PagedResponse<IEnumerable<GetBetDto>>>;