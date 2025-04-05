namespace SportDataService.Application.DTO.Odds;

public sealed class OddsGetDto
{
    public string Id { get; set; }
    public string MatchId { get; set; }
    public string MarketType { get; set; }
    public OddsValueGetDto Values { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsLive { get; set; }
}