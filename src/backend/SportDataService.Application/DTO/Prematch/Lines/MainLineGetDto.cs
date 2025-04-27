namespace SportDataService.Application.DTO.Prematch.Lines;

public sealed class MainLineGetDto
{
    public MarketValueGetDto? Opponent1Win { get; init; }
    public MarketValueGetDto? Opponent2Win { get; init; }
    public MarketValueGetDto? Draw { get; init; }
    public MarketValueGetDto? Opponent1WinOrDraw { get; init; }
    public MarketValueGetDto? Opponent2WinOrDraw { get; init; }
}