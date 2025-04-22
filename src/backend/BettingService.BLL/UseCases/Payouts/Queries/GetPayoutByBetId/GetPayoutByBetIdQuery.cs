using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Payments.Queries.GetPayoutByBetId;

public sealed record GetPayoutByBetIdQuery(Guid BetId) : IRequest<Payout>;