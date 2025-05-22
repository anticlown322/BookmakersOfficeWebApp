using Microsoft.AspNetCore.Identity;
using UserService.Application.Contracts.UseCases.Authentication;
using UserService.Application.DTO.Authentication;
using UserService.Presentation.Controllers;

namespace UserService.Tests.UnitTests.Controllers;

public class AuthenticationControllerTests
{
    private readonly Mock<IRegisterUserUseCase> _registerUserUseCaseMock = new();
    private readonly Mock<ILoginUseCase> _loginUseCaseMock = new();
    private readonly Mock<IRefreshTokenForAuthUseCase> _refreshTokenForAuthUseCaseMock = new();
    private readonly Mock<ILogoutUseCase> _logoutUseCaseMock = new();
    private readonly AuthenticationController _controller;

    public AuthenticationControllerTests()
    {
        _controller = new AuthenticationController(
            _registerUserUseCaseMock.Object,
            _loginUseCaseMock.Object,
            _refreshTokenForAuthUseCaseMock.Object,
            _logoutUseCaseMock.Object);
    }

    [Fact]
    public async Task Register_ShouldReturn201StatusCode_WhenRegistrationSuccessful()
    {
        // Arrange
        var userRegistrationDto = new UserRegistrationDto();
        _registerUserUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserRegistrationDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _controller.Register(userRegistrationDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Login_ShouldReturnOkWithTokensGetDto_WhenLoginSuccessful()
    {
        // Arrange
        var userLoginDto = new UserLoginDto();
        var expectedTokens = new TokensGetDto("access_token", "refresh_token");
        _loginUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserLoginDto>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTokens);

        // Act
        var result = await _controller.Login(userLoginDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedTokens);
    }

    [Fact]
    public async Task Refresh_ShouldReturnOkWithAccessToken_WhenRefreshSuccessful()
    {
        // Arrange
        var tokensRefreshDto = new TokensRefreshDto("access_token", "refresh_token");
        var expectedToken = "new_access_token";
        _refreshTokenForAuthUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<TokensRefreshDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedToken);

        // Act
        var result = await _controller.Refresh(tokensRefreshDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().Be(expectedToken);
    }

    [Fact]
    public async Task Logout_ShouldReturnOk_WhenLogoutSuccessful()
    {
        // Arrange
        var userLogoutDto = new UserLogoutDto();
        _logoutUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserLogoutDto>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Logout(userLogoutDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task Register_ShouldCallUseCaseWithCorrectParameters()
    {
        // Arrange
        var userRegistrationDto = new UserRegistrationDto();
        var cancellationToken = CancellationToken.None;
        _registerUserUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserRegistrationDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        await _controller.Register(userRegistrationDto, cancellationToken);

        // Assert
        _registerUserUseCaseMock.Verify(x =>
            x.ExecuteAsync(userRegistrationDto, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Login_ShouldCallUseCaseWithPopulateExpTrue()
    {
        // Arrange
        var userLoginDto = new UserLoginDto();
        _loginUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserLoginDto>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TokensGetDto("", ""));

        // Act
        await _controller.Login(userLoginDto, CancellationToken.None);

        // Assert
        _loginUseCaseMock.Verify(x =>
            x.ExecuteAsync(It.IsAny<UserLoginDto>(), true, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Logout_ShouldCallUseCaseWithPopulateExpTrue()
    {
        // Arrange
        var userLogoutDto = new UserLogoutDto();
        _logoutUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<UserLogoutDto>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Logout(userLogoutDto, CancellationToken.None);

        // Assert
        _logoutUseCaseMock.Verify(x =>
            x.ExecuteAsync(It.IsAny<UserLogoutDto>(), true, It.IsAny<CancellationToken>()), Times.Once);
    }
}