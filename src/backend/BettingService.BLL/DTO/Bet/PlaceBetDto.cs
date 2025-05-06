namespace BettingService.BLL.DTO.Bet;

public record PlaceBetDto(
    string MatchId,
    decimal Amount,
    string LineType,
    string MarketSelection,
    decimal Odds);