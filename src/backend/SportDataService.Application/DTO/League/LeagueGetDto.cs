namespace SportDataService.Application.DTO.League;

public sealed class LeagueGetDto
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Country { get; set; }
    public string SportType { get; set; }
    public string Season { get; set; }
    public bool IsActive { get; set; }
}