using BettingService.BLL.DTO.Payout;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;

public sealed record RequestPayoutCommand(
    string Username,
    RequestPayoutDto RequestPayoutDto)
    : IRequest<Unit>;