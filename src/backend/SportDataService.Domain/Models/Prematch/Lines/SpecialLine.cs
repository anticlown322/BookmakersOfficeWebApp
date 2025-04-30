using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class SpecialLine : IMarketLine
{
    public MarketValue? EitherOpponent1OrOpponent2 { get; set; }
    public MarketValue? GetValue(string marketSelection)
    {
        return marketSelection switch
        {
            LineMarketTypes.EitherOpponent1OrOpponent2 => EitherOpponent1OrOpponent2,
            _ => null
        };
    }
}