using UserService.Application.Contracts.Services;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Account;

public class SendResetPasswordEmailUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly SendResetPasswordEmailUseCase _sendResetPasswordEmailUseCase;

    public SendResetPasswordEmailUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _sendResetPasswordEmailUseCase = new SendResetPasswordEmailUseCase(
            _usersRepositoryMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_SendsResetPasswordEmail()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithConfirmedEmail();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var resetToken = "password_reset_token";

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(user, ct))
            .ReturnsAsync(resetToken);

        _emailServiceMock
            .Setup(x => x.SendResetPasswordEmailAsync(user.Email, resetToken))
            .Returns(Task.CompletedTask);

        // Act
        await _sendResetPasswordEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.GeneratePasswordResetTokenAsync(user, ct), Times.Once);
        _emailServiceMock.Verify(x => x.SendResetPasswordEmailAsync(user.Email, resetToken), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _sendResetPasswordEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedBeforeUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = "testUser";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _sendResetPasswordEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedAfterUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithConfirmedEmail();
        var username = user.UserName;
        var cts = new CancellationTokenSource();

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Callback(() => cts.Cancel());

        // Act
        Func<Task> act = () => _sendResetPasswordEmailUseCase.ExecuteAsync(username, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_EmailServiceThrows_PropagatesException()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithConfirmedEmail();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var resetToken = "password_reset_token";
        var expectedException = new Exception("Email service error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(user, ct))
            .ReturnsAsync(resetToken);

        _emailServiceMock
            .Setup(x => x.SendResetPasswordEmailAsync(user.Email, resetToken))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _sendResetPasswordEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrows_PropagatesException()
    {
        // Arrange
        var username = "testUser";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _sendResetPasswordEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}