using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.DTO.Account;
using UserService.Application.DTO.Authentication;
using UserService.Application.DTO.Balance;
using UserService.Application.DTO.User;
using UserService.Domain.Models;
using UserService.Domain.RequestFeatures;
using UserService.Infrastructure.Utility;

namespace UserService.Tests.UnitTests.UseCases;

public static class UseCasesTestData
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

    public static PagedList<BalanceTransaction> CreateTestTransactions(int count)
    {
        var transactions = Enumerable.Range(1, count)
            .Select(i => new BalanceTransaction
            {
                Id = i,
                Amount = i * 10m,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            })
            .ToList();

        return new PagedList<BalanceTransaction>(
            transactions,
            count,
            1,
            count);
    }

    public static IEnumerable<TransactionDto> CreateTestTransactionDtos(int count) =>
        Enumerable.Range(1, count)
            .Select(i => new TransactionDto
            {
                Amount = i * 10,
                CreatedAt = DateTime.UtcNow.AddDays(-i)
            });

    public static Domain.Models.User CreateUserWithBalance(decimal initialAmount)
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Balance = new UserBalance
            {
                CurrentAmount = initialAmount,
                LastUpdated = DateTime.UtcNow,
                Transactions = new List<BalanceTransaction>()
            }
        };
    }

    public static Domain.Models.User CreateUserWithoutBalance()
    {
        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Balance = null
        };
    }

    public static UserLoginDto ValidLoginDto => new UserLoginDto
    {
        UserName = "testUser",
        Password = "validPassword123"
    };

    public static JwtSettings ValidJwtSettings = new JwtSettings
    {
        SecretKey = "Sellin' my soul to my dreams and my goals but I won't ever stop until I'm on the stretcher",
        Issuer = "Bookmaker office web app",
        Audience = "bookmaker-api-users",
        ExpiryMinutes = 30
    };

    public static TokensGetDto ValidTokens
    {
        get
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(ValidJwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testUser")
                }),
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature),
                Issuer = ValidJwtSettings.Issuer,
                Audience = ValidJwtSettings.Audience
            };

            var accessToken = tokenHandler.CreateToken(tokenDescriptor);

            string refreshToken = String.Empty;
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                refreshToken = Convert.ToBase64String(randomNumber);
            }

            return new TokensGetDto(
                tokenHandler.WriteToken(accessToken),
                refreshToken);
        }
    }

    public static Domain.Models.User CreateAuthenticatedUser()
    {
        var tokens = ValidTokens;

        return new Domain.Models.User
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testUser",
            Email = "testUser@example.com",
            RefreshToken = tokens.RefreshToken,
            RefreshTokenExpiryTime = DateTime.Now.AddMinutes(30)
        };
    }

    public static UserRegistrationDto ValidRegistrationDto => new()
    {
        UserName = "newUser",
        Email = "new@example.com",
        Password = "P@ssw0rd!",
        Roles = new List<string> { "User" }
    };

    public static Domain.Models.User NewUser => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "newUser",
        Email = "new@example.com",
        Profile = new UserProfile(),
        Balance = new UserBalance()
    };

    public static Domain.Models.User ExistingUser => new()
    {
        Id = Guid.NewGuid().ToString(),
        UserName = "existingUser",
        Email = "existing@example.com"
    };

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