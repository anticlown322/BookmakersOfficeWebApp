using SportDataService.Application.UseCases.Match;

namespace SportDataService.Application.DTO.Match;

public record MatchGetDto(
    string Id,
    string LeagueId,
    TeamShortDto HomeTeam,
    TeamShortDto AwayTeam,
    DateTime StartTime,
    string Status,
    ScoreDto CurrentScore,
    IReadOnlyCollection<EventShortDto> Events,
    DateTime CreatedAt);