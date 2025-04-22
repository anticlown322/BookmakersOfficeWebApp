using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Payments.Queries.GetPayoutById;

public sealed record GetPayoutByIdQuery(Guid Id) : IRequest<Payout>;