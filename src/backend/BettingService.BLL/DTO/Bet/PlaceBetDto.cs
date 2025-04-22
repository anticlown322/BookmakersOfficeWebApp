namespace BettingService.BLL.DTO.Bet;

public record PlaceBetDto(
    string MatchId,
    decimal Amount,
    decimal Odds);