using BettingService.BLL.DTO.Payout;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;
using MediatR;

namespace BettingService.BLL.UseCases.Payments.Queries.GetAllUserPayouts;

public sealed record GetAllUserPayoutsQuery(PayoutParameters Parameters, string Username)
    : IRequest<PagedResponse<IEnumerable<GetPayoutDto>>>;