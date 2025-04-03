namespace SportDataService.Application.DTO.Team;

public sealed class TeamUpdateDto
{
    public string? Name { get; set; }
    public string? ShortName { get; set; }
    public string? Country { get; set; }
    public string? SportType { get; set; }
    public List<string>? PlayerIdsToAdd { get; set; }
    public List<string>? PlayerIdsToRemove { get; set; }
}