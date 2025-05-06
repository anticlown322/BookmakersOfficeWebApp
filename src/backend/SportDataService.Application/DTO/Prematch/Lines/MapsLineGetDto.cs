namespace SportDataService.Application.DTO.Prematch.Lines;

public sealed class MapsLineGetDto
{
    public MarketValueGetDto? Map1HandicapOpponent1 { get; init; }
    public MarketValueGetDto? Map1HandicapOpponent2 { get; init; }
    public MarketValueGetDto? Map2HandicapOpponent1 { get; init; }
    public MarketValueGetDto? Map2HandicapOpponent2 { get; init; }
}