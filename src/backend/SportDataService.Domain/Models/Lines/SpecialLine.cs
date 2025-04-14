using SportDataService.Domain.Models.Markets;

namespace SportDataService.Domain.Models.Lines;

public class SpecialLine
{
    public MarketValue? EitherOpponent1OrOpponent2 { get; set; }
}