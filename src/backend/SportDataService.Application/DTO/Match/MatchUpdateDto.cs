namespace SportDataService.Application.DTO.Match;

public sealed class MatchUpdateDto
{
    public string? LeagueId { get; set; }
    public string? HomeTeamId { get; set; }
    public string? AwayTeamId { get; set; }
    public DateTime? StartTime { get; set; }
    public string? Status { get; set; }

    public ScoreUpdateDto? Score { get; set; }
    public List<string>? EventIdsToAdd { get; set; }
}