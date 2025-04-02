namespace SportDataService.Domain.Models;

public class Event
{
    public string Id { get; set; }
    public string MatchId { get; set; }
    public string Type { get; set; }
    public int Minute { get; set; }
    public string TeamId { get; set; }
    public string? PlayerId { get; set; }
    public string? AdditionalInfo { get; set; }
}