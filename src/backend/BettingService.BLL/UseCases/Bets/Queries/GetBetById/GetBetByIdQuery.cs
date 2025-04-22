using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Queries.GetBetById;

public sealed record GetBetByIdQuery(Guid id) : IRequest<Bet>;