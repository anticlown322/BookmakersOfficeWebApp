using UserService.Application.DTO.Authentication;
using UserService.Application.UseCases.Authentication;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Authentication;

public class LogoutUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly LogoutUseCase _logoutUseCase;

    public LogoutUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _logoutUseCase = new LogoutUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidUser_ClearsRefreshToken()
    {
        // Arrange
        var user = AuthUseCasesTestData.CreateAuthenticatedUser();
        user.RefreshToken = "valid_refresh_token";
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);
        
        var logoutDto = new UserLogoutDto { UserName = user.UserName };
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(logoutDto.UserName, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        await _logoutUseCase.ExecuteAsync(logoutDto, true, ct);

        // Assert
        user.RefreshToken.Should().BeNull();
        user.RefreshTokenExpiryTime.Should().BeOnOrBefore(DateTime.UtcNow);
        
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(logoutDto.UserName, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.UpdateUserAsync(user, ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var logoutDto = new UserLogoutDto { UserName = "nonExistingUser" };
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(logoutDto.UserName, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _logoutUseCase.ExecuteAsync(logoutDto, true, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {logoutDto.UserName} does not exist in the database.");


    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var logoutDto = new UserLogoutDto { UserName = "testUser" };
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _logoutUseCase.ExecuteAsync(logoutDto, true, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var user = AuthUseCasesTestData.CreateAuthenticatedUser();
        var logoutDto = new UserLogoutDto { UserName = user.UserName };
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(logoutDto.UserName, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _logoutUseCase.ExecuteAsync(logoutDto, true, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();
            
        exception.Which.Message.Should().Be(expectedException.Message);

    }
}