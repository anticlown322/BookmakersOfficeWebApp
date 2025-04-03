namespace SportDataService.Application.DTO.Team;

public sealed class TeamCreateDto
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Country { get; set; }
    public string SportType { get; set; }
}