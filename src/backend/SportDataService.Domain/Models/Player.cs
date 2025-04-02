namespace SportDataService.Domain.Models;

public class Player
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string TeamId { get; set; }
    public string? Position { get; set; }
    public int? Number { get; set; }
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; }
}