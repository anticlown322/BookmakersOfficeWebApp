namespace SportDataService.Application.DTO.Player;

public sealed class PlayerCreateDto
{
    public string Name { get; set; }
    public string? TeamId { get; set; }
    public string? Position { get; set; }
    public int? Number { get; set; }
    public DateTime BirthDate { get; set; }
    public string Nationality { get; set; }
}