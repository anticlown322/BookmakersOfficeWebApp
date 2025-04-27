namespace SportDataService.Application.DTO.Prematch;

public sealed class TournamentGetDto
{
    public string Id { get; init; }
    public string TournamentId { get; init; }
    public string Name { get; init; }
    public List<MatchGetDto> Matches { get; init; } = new();
}