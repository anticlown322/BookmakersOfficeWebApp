namespace SportDataService.Domain.Models;

public class Match
{
    public string Id { get; set; }
    public string LeagueId { get; set; }
    public string HomeTeamId { get; set; }
    public string AwayTeamId { get; set; }
    public DateTime StartTime { get; set; }
    public string Status { get; set; }
    public Score CurrentScore { get; set; }
    public List<string> EventIds { get; set; } = new List<string>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}