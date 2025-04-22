using BettingService.BLL.DTO.Bet;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed record PlaceBetCommand(
    string Username,
    PlaceBetDto PlaceBetDto
) : IRequest;