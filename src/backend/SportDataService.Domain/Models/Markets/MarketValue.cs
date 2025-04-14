namespace SportDataService.Domain.Models.Markets;

public class MarketValue
{
    public string? Value { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}