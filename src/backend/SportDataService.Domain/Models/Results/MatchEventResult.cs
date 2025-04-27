namespace SportDataService.Domain.Models.Results;

public class MatchEventResult
{
    public string? MatchEventResultId { get; set; }
    public string? ParentMatchResultId { get; set; }
    public string? EventName { get; set; }
    public ResultStatus Status { get; set; }

    public int Team1TotalScore { get; set; }
    public int Team2TotalScore { get; set; }
    public List<SubScore>? SubScores { get; set; } = new();
}