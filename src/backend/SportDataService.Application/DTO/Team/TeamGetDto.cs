namespace SportDataService.Application.DTO.Team;

public sealed class TeamGetDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Country { get; set; }
    public string SportType { get; set; }
    public IReadOnlyCollection<string> PlayerIds { get; set; } = new List<string>();
}