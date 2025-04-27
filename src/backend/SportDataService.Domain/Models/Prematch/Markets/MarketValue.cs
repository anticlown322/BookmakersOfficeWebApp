namespace SportDataService.Domain.Models.Prematch.Markets;

public class MarketValue
{
    public string? Value { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsActive { get; set; } = true;
}