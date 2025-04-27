using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class MapsLine
{
    public MarketValue? Map1HandicapOpponent1 { get; set; }
    public MarketValue? Map1HandicapOpponent2 { get; set; }
    public MarketValue? Map2HandicapOpponent1 { get; set; }
    public MarketValue? Map2HandicapOpponent2 { get; set; }
}