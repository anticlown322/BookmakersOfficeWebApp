using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.Application.Contracts.Services;
using UserService.Application.DTO.Authentication;
using UserService.Application.UseCases.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Authentication;

public class RefreshTokenForAuthUseCaseTests
{
    private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly RefreshTokenForAuthUseCase _refreshTokenUseCase;

    public RefreshTokenForAuthUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _tokenServiceMock = new Mock<ITokenService>();

        _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
        _jwtSettingsMock.Setup(x => x.Value).Returns(UseCasesTestData.ValidJwtSettings);
        
        _refreshTokenUseCase = new RefreshTokenForAuthUseCase(
            _jwtSettingsMock.Object,
            _usersRepositoryMock.Object,
            _tokenServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidTokens_ReturnsNewAccessToken()
    {
        // Arrange
        var user = UseCasesTestData.CreateAuthenticatedUser();
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, user.RefreshToken);
        var ct = CancellationToken.None;
        var newAccessToken = "new_access_token";

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(user.UserName, ct))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(x => x.CreateAccessToken(user))
            .ReturnsAsync(newAccessToken);

        // Act
        var result = await _refreshTokenUseCase.ExecuteAsync(tokensDto, ct);

        // Assert
        result.Should().BeEquivalentTo(newAccessToken);
        
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(user.UserName, ct), Times.Once);
        _tokenServiceMock.Verify(x => x.CreateAccessToken(user), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsRefreshTokenBadRequest()
    {
        // Arrange
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, UseCasesTestData.ValidTokens.RefreshToken);
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(It.IsAny<string>(), ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _refreshTokenUseCase.ExecuteAsync(tokensDto, ct);

        // Assert
        await act.Should().ThrowAsync<RefreshTokenBadRequest>();
    }

    [Fact]
    public async Task ExecuteAsync_InvalidRefreshToken_ThrowsRefreshTokenBadRequest()
    {
        // Arrange
        var user = UseCasesTestData.CreateAuthenticatedUser();
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, UseCasesTestData.ValidTokens.RefreshToken);
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(user.UserName, ct))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _refreshTokenUseCase.ExecuteAsync(tokensDto, ct);

        // Assert
        await act.Should().ThrowAsync<RefreshTokenBadRequest>();
    }

    [Fact]
    public async Task ExecuteAsync_ExpiredRefreshToken_ThrowsRefreshTokenBadRequest()
    {
        // Arrange
        var user = UseCasesTestData.CreateAuthenticatedUser();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1);
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, user.RefreshToken);
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(user.UserName, ct))
            .ReturnsAsync(user);

        // Act and Assert
        await Assert.ThrowsAsync<RefreshTokenBadRequest>(
            () => _refreshTokenUseCase.ExecuteAsync(tokensDto, ct));
    }

    [Fact]
    public async Task ExecuteAsync_TokenServiceReturnsNull_ThrowsTokenNotCreatedException()
    {
        // Arrange
        var user = UseCasesTestData.CreateAuthenticatedUser();
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, user.RefreshToken);
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(user.UserName, ct))
            .ReturnsAsync(user);

        _tokenServiceMock
            .Setup(x => x.CreateAccessToken(user))
            .ReturnsAsync((string)null);

        // Act
        Func<Task> act = () => _refreshTokenUseCase.ExecuteAsync(tokensDto, ct);

        // Assert
        await act.Should()
            .ThrowAsync<TokenNotCreatedException>()
            .WithMessage($"Cannot create access or refresh token {nameof(tokensDto.AccessToken)}.");

    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, UseCasesTestData.ValidTokens.RefreshToken);
        var ct = new CancellationToken(canceled: true);

        // Act and Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => _refreshTokenUseCase.ExecuteAsync(tokensDto, ct));
    }

    [Fact]
    public async Task ExecuteAsync_InvalidAccessToken_ThrowsSecurityTokenException()
    {
        // Arrange
        var tokensDto = new TokensRefreshDto(UseCasesTestData.ValidTokens.AccessToken, UseCasesTestData.ValidTokens.RefreshToken);
        var ct = CancellationToken.None;

        // Act and Assert
        await Assert.ThrowsAsync<RefreshTokenBadRequest>(
            () => _refreshTokenUseCase.ExecuteAsync(tokensDto, ct));
    }
}