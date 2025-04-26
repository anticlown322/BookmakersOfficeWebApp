using SportDataService.Application.DTO.Team;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.Results;

public class MatchResultGetDto
{
    public string Id { get; set; }
    public string? MatchResultId { get; set; }
    public string? TournamentId { get; set; }
    public string? MatchName { get; set; }

    public TeamGetDto Team1 { get; set; } = new();
    public TeamGetDto Team2 { get; set; } = new();

    public DateTime? ResultTime { get; set; }

    public int Team1TotalScore { get; set; }
    public int Team2TotalScore { get; set; }
    public List<SubScoreGetDto> SubScores { get; set; } = new();
    public List<MatchEventResultGetDto> EventResults { get; set; } = new();

    public ResultStatus Status { get; set; }
}