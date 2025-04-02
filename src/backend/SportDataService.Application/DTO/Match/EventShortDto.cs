namespace SportDataService.Application.DTO.Match;

public record EventShortDto(
    string Id,
    string Type,
    int Minute,
    string TeamId,
    string? PlayerId,
    string? AdditionalInfo);