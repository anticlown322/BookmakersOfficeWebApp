using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.DTO.Lines;

public sealed class SpecialLineGetDto
{
    public MarketValueGetDto? EitherOpponent1OrOpponent2 { get; init; }
}