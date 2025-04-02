namespace SportDataService.Domain.Models;

public class Team
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public string Country { get; set; }
    public string SportType { get; set; }
    public List<string> PlayerIds { get; set; } = new List<string>();
}