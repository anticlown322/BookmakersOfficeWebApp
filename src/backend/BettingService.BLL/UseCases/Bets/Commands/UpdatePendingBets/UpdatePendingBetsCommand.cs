using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.UpdatePendingBets;

public record UpdatePendingBetsCommand : IRequest<Unit>;