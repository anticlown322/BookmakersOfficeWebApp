using Microsoft.AspNetCore.Identity;
using UserService.Application.DTO.Account;
using UserService.Application.UseCases.Account;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Account;

public class ResetPasswordUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly ResetPasswordUseCase _resetPasswordUseCase;

    public ResetPasswordUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _resetPasswordUseCase = new ResetPasswordUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidReset_ResetsPassword()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithPassword();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var passwordResetDto = AccountUseCasesTestData.ValidPasswordResetDto;
        var successResult = IdentityResult.Success;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.ResetPasswordAsync(user, passwordResetDto.Token, passwordResetDto.NewPassword, ct))
            .ReturnsAsync(successResult);

        // Act
        await _resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, ct);

        // Assert
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(
            x => x.ResetPasswordAsync(user, passwordResetDto.Token, passwordResetDto.NewPassword, ct),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;
        var passwordResetDto = AccountUseCasesTestData.ValidPasswordResetDto;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_ResetFails_ThrowsPasswordCanNotBeReset()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithPassword();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var passwordResetDto = new PasswordResetDto("invalid_token", "NewP@ssw0rd123");
        var error = new IdentityError { Description = "Invalid token" };
        var failedResult = IdentityResult.Failed(error);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.ResetPasswordAsync(user, passwordResetDto.Token, passwordResetDto.NewPassword, ct))
            .ReturnsAsync(failedResult);

        // Act
        Func<Task> act = () => _resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, ct);

        // Assert
        await act.Should().ThrowAsync<PasswordCanNotBeReset>();
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = "testUser";
        var ct = new CancellationToken(canceled: true);
        var passwordResetDto = AccountUseCasesTestData.ValidPasswordResetDto;

        // Act
        Func<Task> act = () => _resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var user = AccountUseCasesTestData.CreateUserWithPassword();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var passwordResetDto = AccountUseCasesTestData.ValidPasswordResetDto;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _resetPasswordUseCase.ExecuteAsync(username, passwordResetDto, ct);

        // Assert
        var exception = await act.Should()
            .ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}