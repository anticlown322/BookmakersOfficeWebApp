namespace SportDataService.Application.DTO.Match;

public sealed class MatchCreateDto
{
    public string LeagueId { get; set; }
    public string HomeTeamId { get; set; }
    public string AwayTeamId { get; set; }
    public DateTime StartTime { get; set; }
    public string Status { get; set; } = "Scheduled";
    public ScoreCreateDto? InitialScore { get; set; }
}
