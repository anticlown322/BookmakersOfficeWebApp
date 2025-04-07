using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.DTO.Lines;

public sealed class KillsLineGetDto
{
    public MarketValueGetDto? Opponent1KillsMain { get; init; }
    public MarketValueGetDto? Opponent2KillsMain { get; init; }
    public MarketValueGetDto? TotalKillsUnder { get; init; }
    public MarketValueGetDto? TotalKillsOver { get; init; }
    public MarketValueGetDto? Opponent1KillsHandicap { get; init; }
    public MarketValueGetDto? Opponent2KillsHandicap { get; init; }
}