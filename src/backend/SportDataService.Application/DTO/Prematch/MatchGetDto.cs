using SportDataService.Application.DTO.Common;
using SportDataService.Application.DTO.Prematch.Lines;

namespace SportDataService.Application.DTO.Prematch;

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
