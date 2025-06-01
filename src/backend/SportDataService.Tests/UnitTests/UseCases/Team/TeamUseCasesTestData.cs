using SportDataService.Application.DTO.Common;
using SportDataService.Domain.RequestFeatures;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Tests.UnitTests.UseCases.Team;

public static class TeamUseCasesTestData
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
}