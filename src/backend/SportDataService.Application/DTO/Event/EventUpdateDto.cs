namespace SportDataService.Application.DTO.Event;

public sealed class EventUpdateDto
{
    public string? Type { get; set; }
    public int? Minute { get; set; }
    public string? TeamId { get; set; }
    public string? PlayerId { get; set; }
    public string? AdditionalInfo { get; set; }
}