using BettingService.DAL.Models.Entities;

namespace BettingService.BLL.DTO.Bet;

public record BetDto(
    Guid Id,
    string Username,
    string MatchId,
    decimal Amount,
    decimal Odds,
    BetStatus Status,
    DateTime CreatedAt,
    DateTime? SettledAt);