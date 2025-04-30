using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.UpdateActiveBets;

public record UpdateActiveBetsCommand : IRequest<Unit>;