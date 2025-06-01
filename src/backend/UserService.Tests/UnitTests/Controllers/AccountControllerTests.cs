using Microsoft.AspNetCore.Http;
using UserService.Application.Contracts.UseCases.Account;
using UserService.Application.DTO.Account;
using UserService.Presentation.Controllers;

namespace UserService.Tests.UnitTests.Controllers;

public class AccountControllerTests
{
    private readonly Mock<ISendConfirmationEmailUseCase> _sendConfirmationEmailUseCaseMock = new();
    private readonly Mock<IConfirmEmailUseCase> _confirmEmailUseCaseMock = new();
    private readonly Mock<ISendResetPasswordEmailUseCase> _sendResetPasswordEmailUseCaseMock = new();
    private readonly Mock<IResetPasswordUseCase> _resetPasswordUseCaseMock = new();
    private readonly Mock<IGetUserProfileUseCase> _getUserProfileUseCaseMock = new();
    private readonly Mock<IUpdateUserProfileUseCase> _updateUserProfileUseCaseMock = new();
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _controller = new AccountController(
            _sendConfirmationEmailUseCaseMock.Object,
            _confirmEmailUseCaseMock.Object,
            _sendResetPasswordEmailUseCaseMock.Object,
            _resetPasswordUseCaseMock.Object,
            _getUserProfileUseCaseMock.Object,
            _updateUserProfileUseCaseMock.Object);
    }

    [Fact]
    public async Task ConfirmEmail_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        _confirmEmailUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ConfirmEmail(username, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task SendConfirmationEmail_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("example.com");

        var httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _sendConfirmationEmailUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SendConfirmationEmail(
            username,
            httpContextAccessorMock.Object,
            CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkResult>();
        _sendConfirmationEmailUseCaseMock.Verify(x =>
            x.ExecuteAsync(username, "https://example.com", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendResetEmail_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        _sendResetPasswordEmailUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.SendResetEmail(username, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ResetPassword_ShouldReturnOk_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var passwordResetDto = new PasswordResetDto("token", "newPassword");
        _resetPasswordUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<PasswordResetDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.ResetPassword(username, passwordResetDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task GetUserProfile_ShouldReturnOkWithProfile_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var expectedProfile = new UserProfileGetDto { FirstName = "John", LastName = "Doe" };
        _getUserProfileUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedProfile);

        // Act
        var result = await _controller.GetUserProfile(username, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedProfile);
    }

    [Fact]
    public async Task UpdateUserProfile_ShouldReturnNoContent_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var profileDto = new UserProfileUpdateDto { FirstName = "John", LastName = "Doe" };
        _updateUserProfileUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<UserProfileUpdateDto>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateUserProfile(username, profileDto, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task ConfirmEmail_ShouldCallUseCaseWithCorrectUsername()
    {
        // Arrange
        const string username = "testuser";
        var cancellationToken = CancellationToken.None;
        _confirmEmailUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.ConfirmEmail(username, cancellationToken);

        // Assert
        _confirmEmailUseCaseMock.Verify(x =>
            x.ExecuteAsync(username, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task ResetPassword_ShouldCallUseCaseWithCorrectParameters()
    {
        // Arrange
        const string username = "testuser";
        var passwordResetDto = new PasswordResetDto("token", "newPassword");
        var cancellationToken = CancellationToken.None;
        _resetPasswordUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<PasswordResetDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.ResetPassword(username, passwordResetDto, cancellationToken);

        // Assert
        _resetPasswordUseCaseMock.Verify(x =>
            x.ExecuteAsync(username, passwordResetDto, cancellationToken), Times.Once);
    }
}