using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Commands.ProcessPayouts;

public record ProcessPayoutsCommand : IRequest<Unit>;