using SportDataService.Application.DTO.Common;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;
using ResultStatus = SportDataService.Domain.Models.Results.ResultStatus;

namespace SportDataService.Tests.UnitTests.UseCases;

public static class UseCasesTestData
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
        
        return result;;
    }

    public static IEnumerable<TeamGetDto> CreateTestTeamDtos(int count)
    {
        return Enumerable.Range(1, count)
            .Select(i => new TeamGetDto
            {
                Id = $"team_{i}",
                TeamId = $"500{i}",
                Name = $"Team {i}"
            });
    }
    
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

        return new PagedList<Domain.Models.Results.TournamentResult>(results, count, 1, 10);
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