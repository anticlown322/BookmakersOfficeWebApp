using Microsoft.AspNetCore.Identity;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Account;

public class ConfirmEmailUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly ConfirmEmailUseCase _confirmEmailUseCase;

    public ConfirmEmailUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _confirmEmailUseCase = new ConfirmEmailUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidConfirmation_ConfirmsEmail()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var token = "email_confirmation_token";
        var successResult = IdentityResult.Success;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(user, ct))
            .ReturnsAsync(token);

        _usersRepositoryMock
            .Setup(x => x.ConfirmEmailAsync(user, token, ct))
            .ReturnsAsync(successResult);

        // Act
        await _confirmEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.GenerateEmailConfirmationTokenAsync(user, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.ConfirmEmailAsync(user, token, ct), Times.Once);
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
        var act = () => _confirmEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_EmailAlreadyConfirmed_ThrowsEmailCanNotBeConfirmedException()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithConfirmedEmail();
        var username = user.UserName;
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act
        var act = () => _confirmEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<EmailCanNotBeConfirmedException>()
            .WithMessage("already confirmed");
    }

    [Fact]
    public async Task ExecuteAsync_ConfirmationFails_ThrowsEmailCanNotBeConfirmedException()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var token = "email_confirmation_token";
        var error = new IdentityError { Description = "Invalid token" };
        var failedResult = IdentityResult.Failed(error);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GenerateEmailConfirmationTokenAsync(user, ct))
            .ReturnsAsync(token);

        _usersRepositoryMock
            .Setup(x => x.ConfirmEmailAsync(user, token, ct))
            .ReturnsAsync(failedResult);

        // Act
        var act = () => _confirmEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<EmailCanNotBeConfirmedException>();
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = "testUser";
        var ct = new CancellationToken(canceled: true);

        // Act
        var act = () => _confirmEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithUnconfirmedEmail();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        var act = () => _confirmEmailUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage(expectedException.Message);
    }
}