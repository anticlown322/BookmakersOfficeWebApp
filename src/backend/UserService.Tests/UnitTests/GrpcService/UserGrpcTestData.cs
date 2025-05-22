using UserService.Domain.Models;

namespace UserService.Tests.UnitTests.GrpcService;

public static class UserGrpcTestData
{
    public static User ValidUser => new User
    {
        UserName = "testUser",
        Balance = new UserBalance { CurrentAmount = 100.50m }
    };
}