using BettingService.BLL.DTO.Bet;
using BettingService.DAL.Models.Kafka.BetValidation;
using MediatR;

namespace BettingService.BLL.UseCases.Bets.Commands.PlaceBet;

public sealed record PlaceBetCommand(
    string Username,
    PlaceBetDto PlaceBetDto)
    : IRequest<BetPlacementResult>;