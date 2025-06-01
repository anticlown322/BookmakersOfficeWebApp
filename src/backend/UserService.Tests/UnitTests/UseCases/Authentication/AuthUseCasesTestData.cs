using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.DTO.Authentication;
using UserService.Domain.Models;

namespace UserService.Tests.UnitTests.UseCases.Authentication;

public static class AuthUseCasesTestData
{
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
}