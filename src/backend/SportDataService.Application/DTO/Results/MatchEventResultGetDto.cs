using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.Results;

public class MatchEventResultGetDto
{
    public string MatchEventResultId { get; set; }
    public string? ParentMatchResultId { get; set; }
    public string? EventName { get; set; }
    public ResultStatus Status { get; set; }

    public int Team1TotalScore { get; set; }
    public int Team2TotalScore { get; set; }
    public List<SubScoreGetDto> SubScores { get; set; } = new();
}