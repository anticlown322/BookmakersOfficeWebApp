using SportDataService.Domain.Models.Common;

namespace SportDataService.Domain.Models.Results;

public class MatchResult
{
    public string Id { get; set; }
    public string? MatchResultId { get; set; }
    public string? TournamentId { get; set; }
    public string? MatchName { get; set; }

    public Team Team1 { get; set; } = new();
    public Team Team2 { get; set; } = new();

    public DateTime? ResultTime { get; set; }

    public int Team1TotalScore { get; set; }
    public int Team2TotalScore { get; set; }
    public List<SubScore> SubScores { get; set; } = new();
    public List<MatchEventResult> EventResults { get; set; } = new();

    public ResultStatus Status { get; set; }
}