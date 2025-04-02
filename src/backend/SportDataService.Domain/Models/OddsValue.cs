namespace SportDataService.Domain.Models;

public class OddsValue
{
    public decimal Home { get; set; }
    public decimal? Draw { get; set; } 
    public decimal Away { get; set; }
}