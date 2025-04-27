using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public class MainLine
{
    public MarketValue? Opponent1Win { get; set; }
    public MarketValue? Opponent2Win { get; set; }
    public MarketValue? Draw { get; set; }
    public MarketValue? Opponent1WinOrDraw { get; set; }
    public MarketValue? Opponent2WinOrDraw { get; set; }
}