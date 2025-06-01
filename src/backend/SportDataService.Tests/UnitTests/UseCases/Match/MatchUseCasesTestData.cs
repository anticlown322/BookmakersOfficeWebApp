using SportDataService.Application.DTO.Common;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Tests.UnitTests.UseCases.Match;

public static class MatchUseCasesTestData
{
    public static PagedList<Domain.Models.Prematch.Match> CreateTestMatchesWithMetadata(int count)
    {
        var matches = Enumerable.Range(1, count)
            .Select(i => new Domain.Models.Prematch.Match
            {
                Id = Guid.NewGuid().ToString(),
                MatchId = $"Match_{i}",
                TournamentId = $"Tournament_{i}",
                Opponent1 = new Domain.Models.Common.Team { Id = Guid.NewGuid().ToString(), Name = $"TeamA_{i}" },
                Opponent2 = new Domain.Models.Common.Team { Id = Guid.NewGuid().ToString(), Name = $"TeamB_{i}" },
                StartTime = DateTime.UtcNow.AddDays(i)
            })
            .ToList();

        var result = new PagedList<Domain.Models.Prematch.Match>(matches, count, 1, 10)
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

    public static IEnumerable<MatchGetDto> CreateTestMatchDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new MatchGetDto
            {
                Id = Guid.NewGuid().ToString(),
                MatchId = $"Match_{i}",
                TournamentId = $"Tournament_{i}",
                Opponent1 = new TeamGetDto { Id = Guid.NewGuid().ToString(), Name = $"TeamA_{i}" },
                Opponent2 = new TeamGetDto { Id = Guid.NewGuid().ToString(), Name = $"TeamB_{i}" },
                StartTime = DateTime.UtcNow.AddDays(i)
            });
    }
}