using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.DTO.Tournament;

public sealed class TournamentGetDto
{
    public string Id { get; init; }
    public string TournamentId { get; init; }
    public string Name { get; init; }
    public List<MatchGetDto> Matches { get; init; } = new();
}