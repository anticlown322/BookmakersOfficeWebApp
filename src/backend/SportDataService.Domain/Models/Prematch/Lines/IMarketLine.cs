using SportDataService.Domain.Models.Prematch.Markets;

namespace SportDataService.Domain.Models.Prematch.Lines;

public interface IMarketLine
{
    MarketValue? GetValue(string marketSelection);
}