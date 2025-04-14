using SportDataService.Domain.Models.Lines;

namespace SportDataService.Domain.Models.Tournaments;

public class Match
{
    public string Id { get; set; }
    public string? MatchId { get; set; }
    public string? TournamentId { get; set; }

    public Team Opponent1 { get; set; } = new();
    public Team Opponent2 { get; set; } = new();

    public DateTime? StartTime { get; set; }

    public MainLine MainLine { get; set; } = new();
    public KillsLine KillsLine { get; set; } = new();
    public MapsLine MapsLine { get; set; } = new();
    public SpecialLine SpecialLine { get; set; } = new();
}