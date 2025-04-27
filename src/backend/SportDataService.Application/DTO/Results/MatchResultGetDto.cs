using SportDataService.Application.DTO.Common;
using SportDataService.Domain.Models.Results;

namespace SportDataService.Application.DTO.Results;

public class MatchResultGetDto
{
    public string Id { get; init; }
    public string? MatchResultId { get; init; }
    public string? TournamentId { get; init; }
    public string? MatchName { get; init; }
    public TeamGetDto Team1 { get; init; }
    public TeamGetDto Team2 { get; init; }

    public DateTime? ResultTime { get; init; }

    public int Team1TotalScore { get; init; }
    public int Team2TotalScore { get; init; }
    public List<SubScoreGetDto> SubScores { get; init; } = new();
    public List<MatchEventResultGetDto> EventResults { get; init; } = new();

    public ResultStatus Status { get; init; }
}