namespace SportDataService.Application.DTO.Odds;

public sealed class OddsCreateDto
{
    public string MatchId { get; set; }
    public string MarketType { get; set; }
    public OddsValueCreateDto Values { get; set; }
    public bool IsLive { get; set; }
}