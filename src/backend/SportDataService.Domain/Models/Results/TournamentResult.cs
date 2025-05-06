namespace SportDataService.Domain.Models.Results;

public class TournamentResult
{
    public string Id { get; set; }
    public string? TournamentId { get; set; }
    public string? TournamentName { get; set; }
    public List<MatchResult> Matches { get; set; } = [];
}