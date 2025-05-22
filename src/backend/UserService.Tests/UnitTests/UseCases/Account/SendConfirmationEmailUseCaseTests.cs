using UserService.Application.Contracts.Services;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Account;

public class SendConfirmationEmailUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IEmailService> _emailServiceMock;
    private readonly SendConfirmationEmailUseCase _sendConfirmationEmailUseCase;

    public SendConfirmationEmailUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _emailServiceMock = new Mock<IEmailService>();
        _sendConfirmationEmailUseCase = new SendConfirmationEmailUseCase(
            _usersRepositoryMock.Object,
            _emailServiceMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_SendsConfirmationEmail()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var baseUrl = "https://example.com";
        var expectedLink = $"{baseUrl}/api/users/{username}/account/confirm-email";
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _emailServiceMock
            .Setup(x => x.SendConfirmationEmailAsync(user.Email, expectedLink))
            .Returns(Task.CompletedTask);

        // Act
        await _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct);

        // Assert
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _emailServiceMock.Verify(x => x.SendConfirmationEmailAsync(user.Email, expectedLink), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var baseUrl = "https://example.com";
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");

    }

    [Fact]
    public async Task ExecuteAsync_EmailAlreadyConfirmed_ThrowsEmailCanNotBeConfirmedException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithConfirmedEmail();
        var username = user.UserName;
        var baseUrl = "https://example.com";
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act and Assert
        await Assert.ThrowsAsync<EmailCanNotBeConfirmedException>(
            () => _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct));
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedBeforeUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = "testUser";
        var baseUrl = "https://example.com";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedAfterUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var baseUrl = "https://example.com";
        var cts = new CancellationTokenSource();

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Callback(() => cts.Cancel());

        // Act
        Func<Task> act = () => _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_EmailServiceThrows_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var baseUrl = "https://example.com";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Email service error");
        var expectedLink = $"{baseUrl}/api/users/{username}/account/confirm-email";

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _emailServiceMock
            .Setup(x => x.SendConfirmationEmailAsync(user.Email, expectedLink))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();
            
        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrows_PropagatesException()
    {
        // Arrange
        var username = "testUser";
        var baseUrl = "https://example.com";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct);

        // Assert
        var exception = await act.Should()
            .ThrowAsync<Exception>();
            
        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_GeneratesCorrectConfirmationLink()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var baseUrl = "https://test.com";
        var expectedLink = $"{baseUrl}/api/users/{username}/account/confirm-email";
        var ct = CancellationToken.None;
        string actualLink = null;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _emailServiceMock
            .Setup(x => x.SendConfirmationEmailAsync(user.Email, It.IsAny<string>()))
            .Callback<string, string>((email, link) => actualLink = link)
            .Returns(Task.CompletedTask);

        // Act
        await _sendConfirmationEmailUseCase.ExecuteAsync(username, baseUrl, ct);

        // Assert
        Assert.Equal(expectedLink, actualLink);
    }
}