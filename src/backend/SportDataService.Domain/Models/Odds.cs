namespace SportDataService.Domain.Models;

public class Odds
{
    public string Id { get; set; }
    public string MatchId { get; set; }
    public string MarketType { get; set; }
    public OddsValue Values { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public bool IsLive { get; set; }
}
