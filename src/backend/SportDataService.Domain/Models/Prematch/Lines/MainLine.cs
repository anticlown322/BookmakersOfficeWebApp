using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class MainLine : IMarketLine
{
    public MarketValue? Opponent1Win { get; set; }
    public MarketValue? Opponent2Win { get; set; }
    public MarketValue? Draw { get; set; }
    public MarketValue? Opponent1WinOrDraw { get; set; }
    public MarketValue? Opponent2WinOrDraw { get; set; }

    public MarketValue? GetValue(string marketSelection)
    {
        return marketSelection switch
        {
            LineMarketTypes.Opponent1Win => Opponent1Win,
            LineMarketTypes.Opponent2Win => Opponent2Win,
            LineMarketTypes.Draw => Draw,
            LineMarketTypes.Opponent1WinOrDraw => Opponent1WinOrDraw,
            LineMarketTypes.Opponent2WinOrDraw => Opponent2WinOrDraw,
            _ => null
        };
    }
}