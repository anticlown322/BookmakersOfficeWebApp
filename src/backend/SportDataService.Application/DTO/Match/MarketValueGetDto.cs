namespace SportDataService.Application.DTO.Match;

public sealed class MarketValueGetDto
{
    public string? Value { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}