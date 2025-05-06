using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class MapsLine : IMarketLine
{
    public MarketValue? Map1HandicapOpponent1 { get; set; }
    public MarketValue? Map1HandicapOpponent2 { get; set; }
    public MarketValue? Map2HandicapOpponent1 { get; set; }
    public MarketValue? Map2HandicapOpponent2 { get; set; }
    public MarketValue? GetValue(string marketSelection)
    {
        return marketSelection switch
        {
            LineMarketTypes.Map1HandicapOpponent1 => Map1HandicapOpponent1,
            LineMarketTypes.Map1HandicapOpponent2 => Map1HandicapOpponent2,
            LineMarketTypes.Map2HandicapOpponent1 => Map2HandicapOpponent1,
            LineMarketTypes.Map2HandicapOpponent2 => Map2HandicapOpponent2,
            _ => null
        };
    }
}