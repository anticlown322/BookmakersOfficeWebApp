namespace SportDataService.Application.DTO.Odds;

public sealed class OddsUpdateDto
{
    public string? MarketType { get; set; }
    public OddsValueUpdateDto? Values { get; set; }
    public bool? IsLive { get; set; }
}