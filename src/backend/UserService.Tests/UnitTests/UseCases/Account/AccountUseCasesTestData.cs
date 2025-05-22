using UserService.Application.DTO.Account;
using UserService.Domain.Models;
using UserService.Infrastructure.Utility;

namespace UserService.Tests.UnitTests.UseCases.Account;

public static class AccountUseCasesTestData
{
    public static Domain.Models.User CreateUserWithProfile()
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Profile = new UserProfile
            {
                FirstName = "Test",
                LastName = "User",
                Roles = new List<string> { UserRoles.Administrator, UserRoles.Gambler }
            },
        };
    }

    public static UserProfileGetDto ValidUserProfileDto => new UserProfileGetDto
    {
        FirstName = "Test",
        LastName = "User",
        Roles = new List<string> { UserRoles.Administrator, UserRoles.Gambler }
    };

    public static Domain.Models.User CreateUserWithUnconfirmedEmail()
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "test@example.com",
            EmailConfirmed = false
        };
    }

    public static Domain.Models.User CreateUserWithConfirmedEmail()
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "test@example.com",
            EmailConfirmed = true
        };
    }

    public static Domain.Models.User CreateUserWithPassword()
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "test@example.com",
            EmailConfirmed = true
        };
    }

    public static PasswordResetDto ValidPasswordResetDto => new PasswordResetDto
    (
        "valid_reset_token",
        "ValidP@ssw0rd123"
    );
}