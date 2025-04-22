using BettingService.BLL.DTO.Payout;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;
using MediatR;

namespace BettingService.BLL.UseCases.Payments.Queries.GetAllPayouts;

public sealed record GetAllPayoutsQuery(PayoutParameters Parameters)
    : IRequest<PagedResponse<IEnumerable<GetPayoutDto>>>;