namespace SportDataService.Domain.Models.Tournaments;

public class Tournament
{
    public string Id { get; set; }
    public string? TournamentId { get; set; }
    public string? Name { get; set; }
    public List<Match> Matches { get; set; } = new();
}