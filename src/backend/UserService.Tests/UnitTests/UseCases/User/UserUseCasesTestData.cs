using UserService.Application.DTO.User;
using UserService.Domain.Models;
using UserService.Domain.RequestFeatures;

namespace UserService.Tests.UnitTests.UseCases.User;

public static class UserUseCasesTestData
{
    public static Guid ValidUserId = Guid.NewGuid();

    public static Domain.Models.User ValidUser => new()
    {
        Id = ValidUserId.ToString(),
        UserName = "testUser",
        Balance = new UserBalance { CurrentAmount = 100.50m }
    };

    public static UserGetDto ValidUserDto => new()
    {
        UserName = "testUser"
    };

    public static PagedList<Domain.Models.User> CreateTestUsers(int count)
    {
        var users = Enumerable.Range(1, count)
            .Select(i => new Domain.Models.User { Id = i.ToString(), UserName = $"user{i}" })
            .ToList();

        return new PagedList<Domain.Models.User>(
            users,
            count,
            1,
            count);
    }

    public static IEnumerable<UserGetDto> CreateTestUserDtos(int count) =>
        Enumerable.Range(1, count)
            .Select(i => new UserGetDto { UserName = $"user{i}" });
}