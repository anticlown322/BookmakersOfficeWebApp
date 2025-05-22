using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.Services;
using UserService.Application.DTO.Authentication;
using UserService.Application.UseCases.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace UserService.Tests.UnitTests.UseCases.Authentication;

public class LoginUseCaseTests
{
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<SignInManager<Domain.Models.User>> _signInManagerMock;
    private readonly LoginUseCase _loginUseCase;

    public LoginUseCaseTests()
    {
        _tokenServiceMock = new Mock<ITokenService>();
        _usersRepositoryMock = new Mock<IUsersRepository>();

        var userManagerMock = new Mock<UserManager<Domain.Models.User>>(
            Mock.Of<IUserStore<Domain.Models.User>>(),
            null!, null!, null!, null!, null!, null!, null!, null!);
        _signInManagerMock = new Mock<SignInManager<Domain.Models.User>>(
            userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<Domain.Models.User>>(),
            null!, null!, null!, null!);

        _loginUseCase = new LoginUseCase(
            _tokenServiceMock.Object,
            _usersRepositoryMock.Object,
            _signInManagerMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidCredentials_ReturnsTokens()
    {
        // Arrange
        var user = AuthUseCasesTestData.CreateAuthenticatedUser();
        var loginDto = AuthUseCasesTestData.ValidLoginDto;
        var ct = CancellationToken.None;
        var expectedTokens = new TokensGetDto("access_token", "refresh_token");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(loginDto.UserName, ct))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _tokenServiceMock
            .Setup(x => x.CreateTokens(user, It.IsAny<bool>()))
            .ReturnsAsync(expectedTokens);

        // Act
        var result = await _loginUseCase.ExecuteAsync(loginDto, true, ct);

        // Assert
        result.AccessToken.Should().Be(expectedTokens.AccessToken);
        result.RefreshToken.Should().Be(expectedTokens.RefreshToken);

        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(loginDto.UserName, ct), Times.Once);
        _signInManagerMock.Verify(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false), Times.Once);
        _tokenServiceMock.Verify(x => x.CreateTokens(user, true), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var loginDto = AuthUseCasesTestData.ValidLoginDto;
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(loginDto.UserName, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _loginUseCase.ExecuteAsync(loginDto, true, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {loginDto.UserName} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_InvalidPassword_ThrowsInvalidCredentialsException()
    {
        // Arrange
        var user = AuthUseCasesTestData.CreateAuthenticatedUser();
        var loginDto = new UserLoginDto { UserName = user.UserName, Password = "invalidPass" };
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(loginDto.UserName, ct))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        Func<Task> act = () => _loginUseCase.ExecuteAsync(loginDto, true, ct);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidCredentialsException>()
            .WithMessage($"Cannot login with username {loginDto.UserName} and password {loginDto.Password}.");
    }

    [Fact]
    public async Task ExecuteAsync_TokenServiceReturnsNullAccessToken_ThrowsTokenNotCreatedException()
    {
        // Arrange
        var user = AuthUseCasesTestData.CreateAuthenticatedUser();
        var loginDto = AuthUseCasesTestData.ValidLoginDto;
        var ct = CancellationToken.None;
        var invalidTokens = new TokensGetDto(null, "refresh_token");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(loginDto.UserName, ct))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _tokenServiceMock
            .Setup(x => x.CreateTokens(user, It.IsAny<bool>()))
            .ReturnsAsync(invalidTokens);

        // Act
        Func<Task> act = () => _loginUseCase.ExecuteAsync(loginDto, true, ct);

        // Assert
        await act.Should()
            .ThrowAsync<TokenNotCreatedException>()
            .WithMessage($"Cannot create access or refresh token {nameof(invalidTokens.AccessToken)}.");
    }

    [Fact]
    public async Task ExecuteAsync_TokenServiceReturnsNullRefreshToken_ThrowsTokenNotCreatedException()
    {
        // Arrange
        var user = AuthUseCasesTestData.CreateAuthenticatedUser();
        var loginDto = AuthUseCasesTestData.ValidLoginDto;
        var ct = CancellationToken.None;
        var invalidTokens = new TokensGetDto("access_token", null);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(loginDto.UserName, ct))
            .ReturnsAsync(user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(user, loginDto.Password, false))
            .ReturnsAsync(SignInResult.Success);

        _tokenServiceMock
            .Setup(x => x.CreateTokens(user, It.IsAny<bool>()))
            .ReturnsAsync(invalidTokens);

        // Act
        Func<Task> act = () => _loginUseCase.ExecuteAsync(loginDto, true, ct);

        // Assert
        await act.Should()
            .ThrowAsync<TokenNotCreatedException>()
            .WithMessage($"Cannot create access or refresh token {nameof(invalidTokens.RefreshToken)}.");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var loginDto = AuthUseCasesTestData.ValidLoginDto;
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _loginUseCase.ExecuteAsync(loginDto, true, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }
}