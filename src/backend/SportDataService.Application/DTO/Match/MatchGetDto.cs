using SportDataService.Application.DTO.Lines;
using SportDataService.Application.DTO.Team;

namespace SportDataService.Application.DTO.Match;

public sealed class MatchGetDto
{
    public string Id { get; init; }
    public string MatchId { get; init; }
    public string TournamentId { get; init; }
    public TeamGetDto Opponent1 { get; init; }
    public TeamGetDto Opponent2 { get; init; }
    public DateTime StartTime { get; init; }
    public MainLineGetDto? MainLine { get; init; }
    public KillsLineGetDto? KillsLine { get; init; }
    public MapsLineGetDto? MapsLine { get; init; }
    public SpecialLineGetDto? SpecialLine { get; init; }
}
