using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.Infrastructure.Services;

namespace UserService.Tests.UnitTests.Services;

public class TokenServiceTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock = new();
    private readonly JwtSettings _jwtSettings = new()
    {
        SecretKey = "SuperSecretKey1234567890GodDamnOppsWon'tFindIt",
        Issuer = "TestIssuer",
        Audience = "TestAudience",
        ExpiryMinutes = 30
    };

    private readonly TokenService _tokenService;

    public TokenServiceTests()
    {
        var optionsMock = new Mock<IOptions<JwtSettings>>();
        optionsMock.Setup(x => x.Value).Returns(_jwtSettings);

        _tokenService = new TokenService(_usersRepositoryMock.Object, optionsMock.Object);
    }

    [Fact]
    public async Task CreateTokens_ShouldReturnValidTokensAndUpdateUser()
    {
        // Arrange
        var user = new User { UserName = "testUser" };
        var roles = new List<string> { "User", "Admin" };
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserRolesAsync(user, ct))
            .ReturnsAsync(roles);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _tokenService.CreateTokens(user, populateExp: true);

        // Assert
        result.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();

        user.RefreshToken.Should().Be(result.RefreshToken);
        user.RefreshTokenExpiryTime.Should().BeCloseTo(DateTime.UtcNow.AddDays(1), TimeSpan.FromSeconds(1));

        _usersRepositoryMock.Verify(x => x.UpdateUserAsync(user, ct), Times.Once);
    }

    [Fact]
    public async Task CreateAccessToken_ShouldContainCorrectClaims()
    {
        // Arrange
        var user = new User { UserName = "testUser" };
        var roles = new List<string> { "User" };
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserRolesAsync(user, ct))
            .ReturnsAsync(roles);

        // Act
        var token = await _tokenService.CreateAccessToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "testUser");
        jwtToken.Claims.Should().Contain(c => c.Type == ClaimTypes.Role && c.Value == "User");
    }
}