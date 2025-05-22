using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Tests.UnitTests.UseCases.TournamentResult;

public static class TournamentResultUseCasesTestData
{
    public static PagedList<Domain.Models.Common.Team> CreateTestTeamsWithMetadata(int count)
    {
        var teams = Enumerable.Range(1, count)
            .Select(i => new Domain.Models.Common.Team
            {
                Id = $"team_{i}",
                TeamId = $"500{i}",
                Name = $"Team {i}"
            })
            .ToList();

        var metaData = new MetaData
        {
            CurrentPage = 1,
            TotalPages = 1,
            PageSize = 10,
            TotalCount = count
        };

        var result = new PagedList<Domain.Models.Common.Team>(teams, count, 1, 10)
        {
            MetaData = metaData
        };
        
        return result;
    }
    
    public static PagedList<Domain.Models.Results.TournamentResult> CreateTestTournamentResultsWithMetadata(int count)
    {
        var results = Enumerable.Range(1, count)
            .Select(i => new Domain.Models.Results.TournamentResult
            {
                Id = $"tournament_result_{i}",
                TournamentId = $"100{i}",
                TournamentName = $"Tournament {i}",
                Matches = new List<Domain.Models.Results.MatchResult>
                {
                    new Domain.Models.Results.MatchResult
                    {
                        Id = $"match_result_{i}",
                        MatchResultId = $"500{i}",
                        MatchName = $"Match {i}"
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

        var result = new PagedList<Domain.Models.Results.TournamentResult>(results, count, 1, 10)
        {
            MetaData = metaData
        };
        
        return result;
    }

    public static IEnumerable<TournamentResultGetDto> CreateTestTournamentResultDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new TournamentResultGetDto
            {
                Id = $"tournament_result_{i}",
                TournamentId = $"100{i}",
                TournamentName = $"Tournament {i}",
                Matches = new List<MatchResultGetDto>
                {
                    new MatchResultGetDto
                    {
                        Id = $"match_result_{i}",
                        MatchResultId = $"500{i}",
                        MatchName = $"Match {i}"
                    }
                }
            });
    }
}