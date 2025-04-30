using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class KillsLine : IMarketLine
{
    public MarketValue? Opponent1KillsMain { get; set; }
    public MarketValue? Opponent2KillsMain { get; set; }
    public MarketValue? TotalKillsUnder { get; set; }
    public MarketValue? TotalKillsOver { get; set; }
    public MarketValue? Opponent1KillsHandicap { get; set; }
    public MarketValue? Opponent2KillsHandicap { get; set; }
    public MarketValue? GetValue(string marketSelection)
    {
        return marketSelection switch
        {
            LineMarketTypes.Opponent1KillsMain => Opponent1KillsMain,
            LineMarketTypes.Opponent2KillsMain => Opponent2KillsMain,
            LineMarketTypes.TotalKillsUnder => TotalKillsUnder,
            LineMarketTypes.TotalKillsOver => TotalKillsOver,
            LineMarketTypes.Opponent1KillsHandicap => Opponent1KillsHandicap,
            LineMarketTypes.Opponent2KillsHandicap => Opponent2KillsHandicap,
            _ => null
        };
    }
}