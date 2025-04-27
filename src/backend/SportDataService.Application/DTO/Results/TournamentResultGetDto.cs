namespace SportDataService.Application.DTO.Results;

public class TournamentResultGetDto
{
    public string Id { get; set; }
    public string? TournamentId { get; set; }
    public string? TournamentName { get; set; }
    public List<MatchResultGetDto> Matches { get; set; } = [];
}