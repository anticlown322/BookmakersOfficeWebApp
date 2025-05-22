using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Tests.UnitTests.UseCases.Tournament;

public static class TournamentUseCasesTestData
{
    public static PagedList<Domain.Models.Prematch.Tournament> CreateTestTournamentsWithMetadata(int count)
    {
        var tournaments = Enumerable.Range(1, count)
            .Select(i => new Domain.Models.Prematch.Tournament
            {
                Id = $"tournament_{i}",
                TournamentId = $"1000{i}",
                Name = $"Tournament {i}",
                Matches = new List<Domain.Models.Prematch.Match>
                {
                    new Domain.Models.Prematch.Match
                    {
                        Id = $"match_{i}",
                        MatchId = $"500{i}",
                        StartTime = DateTime.UtcNow.AddDays(i)
                    }
                }
            })
            .ToList();

        var metaData = new MetaData
        {
            CurrentPage = 1,
            TotalPages = 1,
            PageSize = 10,
            TotalCount = count
        };

        var result = new PagedList<Domain.Models.Prematch.Tournament>(tournaments, count, 1, 10)
        {
            MetaData = metaData
        };
        
        return result;
    }

    public static IEnumerable<TournamentGetDto> CreateTestTournamentDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new TournamentGetDto
            {
                Id = $"tournament_{i}",
                TournamentId = $"1000{i}",
                Name = $"Tournament {i}",
                Matches = new List<MatchGetDto>
                {
                    new MatchGetDto
                    {
                        Id = $"match_{i}",
                        MatchId = $"500{i}",
                        StartTime = DateTime.UtcNow.AddDays(i)
                    }
                }
            });
    }
}