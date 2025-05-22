using SportDataService.Application.DTO.Common;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;
using ResultStatus = SportDataService.Domain.Models.Results.ResultStatus;

namespace SportDataService.Tests.UnitTests.UseCases.MatchResult;

public static class MatchResultUseCasesTestData
{
    public static PagedList<Domain.Models.Results.MatchResult> CreateTestMatchResultsWithMetadata(int count)
    {
        var results = Enumerable.Range(1, count)
            .Select(i => new Domain.Models.Results.MatchResult
            {
                Id = $"result_{i}",
                MatchResultId = $"50012{i}",
                TournamentId = $"tournament_{i}",
                MatchName = $"Match {i}",
                Team1 = new Domain.Models.Common.Team { Id = $"team1_{i}", Name = $"Team A{i}" },
                Team2 = new Domain.Models.Common.Team { Id = $"team2_{i}", Name = $"Team B{i}" },
                ResultTime = DateTime.UtcNow.AddHours(-i),
                Team1TotalScore = i,
                Team2TotalScore = i + 1,
                SubScores = new List<SubScore>
                {
                    new SubScore { SubscorePosition = 1, Title = "Map 1", Team1Score = i, Team2Score = i }
                },
                EventResults = new List<MatchEventResult>
                {
                    new MatchEventResult { MatchEventResultId = $"event_{i}", EventName = $"Event {i}" }
                },
                Status = i % 2 == 0 ? ResultStatus.Ended : ResultStatus.Running
            })
            .ToList();

        var result = new PagedList<Domain.Models.Results.MatchResult>(results, count, 1, 10)
        {
            MetaData = new MetaData
            {
                CurrentPage = 1,
                TotalPages = 1,
                PageSize = 10,
                TotalCount = count
            }
        };

        return result;
    }

    public static IEnumerable<MatchResultGetDto> CreateTestMatchResultDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new MatchResultGetDto
            {
                Id = $"result_{i}",
                MatchResultId = $"50012{i}",
                TournamentId = $"tournament_{i}",
                MatchName = $"Match {i}",
                Team1 = new TeamGetDto { Id = $"team1_{i}", Name = $"Team A{i}" },
                Team2 = new TeamGetDto { Id = $"team2_{i}", Name = $"Team B{i}" },
                ResultTime = DateTime.UtcNow.AddHours(-i),
                Team1TotalScore = i,
                Team2TotalScore = i + 1,
                SubScores = new List<SubScoreGetDto>
                {
                    new SubScoreGetDto { SubscorePosition = 1, Title = "Map 1", Team1Score = i, Team2Score = i }
                },
                EventResults = new List<MatchEventResultGetDto>
                {
                    new MatchEventResultGetDto { Id = $"event_{i}", EventName = $"Event {i}" }
                },
                Status = i % 2 == 0 ? ResultStatus.Ended : ResultStatus.Running
            });
    }
}