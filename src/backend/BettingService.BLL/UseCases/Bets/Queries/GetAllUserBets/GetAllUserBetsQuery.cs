using BettingService.BLL.DTO.Bet;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Queries.GetAllUserBets;

public sealed record GetAllUserBetsQuery(BetParameters Parameters, string Username)
    : IRequest<PagedResponse<IEnumerable<GetBetDto>>>;