namespace SportDataService.Application.DTO.Match;

public sealed class MatchGetDto
{
    public string Id { get; set; }
    public string LeagueId { get; set; }
    public string HomeTeamId { get; set; }
    public string AwayTeamId { get; set; }
    public DateTime StartTime { get; set; }
    public string Status { get; set; }
    public ScoreGetDto CurrentScoreGet { get; set; }
    public IReadOnlyCollection<string> EventIds { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}