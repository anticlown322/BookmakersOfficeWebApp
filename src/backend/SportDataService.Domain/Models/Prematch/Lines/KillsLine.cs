using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class KillsLine
{
    public MarketValue? Opponent1KillsMain { get; set; }
    public MarketValue? Opponent2KillsMain { get; set; }
    public MarketValue? TotalKillsUnder { get; set; }
    public MarketValue? TotalKillsOver { get; set; }
    public MarketValue? Opponent1KillsHandicap { get; set; }
    public MarketValue? Opponent2KillsHandicap { get; set; }
}