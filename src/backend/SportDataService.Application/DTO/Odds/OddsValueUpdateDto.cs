namespace SportDataService.Application.DTO.Odds;

public sealed class OddsValueUpdateDto
{
    public decimal? Home { get; set; }
    public decimal? Draw { get; set; }
    public decimal? Away { get; set; }
}