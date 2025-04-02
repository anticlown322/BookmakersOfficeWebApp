namespace SportDataService.Application.DTO.Match;

public record TeamShortDto(
    string Id,
    string Name,
    string? LogoUrl = null);