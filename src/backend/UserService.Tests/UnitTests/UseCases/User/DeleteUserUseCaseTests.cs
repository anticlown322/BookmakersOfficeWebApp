using UserService.Application.UseCases.User;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.User;

public class DeleteUserUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly DeleteUserUseCase _deleteUserUseCase;

    public DeleteUserUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _deleteUserUseCase = new DeleteUserUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_UserExists_DeletesUser()
    {
        // Arrange
        const string userName = "testUser";
        var ct = CancellationToken.None;
        var user = UseCasesTestData.ValidUser;
        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(userName, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.DeleteUserAsync(user))
            .Returns(Task.CompletedTask);

        // Act
        await _deleteUserUseCase.ExecuteAsync(userName, ct);

        // Assert
        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(userName, ct),
            Times.Once);

        _usersRepositoryMock.Verify(
            x => x.DeleteUserAsync(user),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotExists_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        const string userName = "unknownUser";
        var ct = CancellationToken.None;
        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(userName, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _deleteUserUseCase.ExecuteAsync(userName, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {userName} does not exist in the database.");
        
        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(userName, ct),
            Times.Once);

        _usersRepositoryMock.Verify(
            x => x.DeleteUserAsync(It.IsAny<Domain.Models.User>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedBeforeExecution_ThrowsOperationCanceledException()
    {
        // Arrange
        const string userName = "testUser";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _deleteUserUseCase.ExecuteAsync(userName, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(It.IsAny<string>(), CancellationToken.None),
            Times.Never);

        _usersRepositoryMock.Verify(
            x => x.DeleteUserAsync(It.IsAny<Domain.Models.User>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedAfterGetUser_ThrowsOperationCanceledException()
    {
        // Arrange
        const string userName = "testUser";
        var cts = new CancellationTokenSource();

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(userName, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UseCasesTestData.ValidUser)
            .Callback(() => cts.Cancel());

        // Act
        Func<Task> act = () => _deleteUserUseCase.ExecuteAsync(userName, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(userName, It.IsAny<CancellationToken>()),
            Times.Once);

        _usersRepositoryMock.Verify(
            x => x.DeleteUserAsync(It.IsAny<Domain.Models.User>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        const string userName = "testUser";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(userName, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _deleteUserUseCase.ExecuteAsync(userName, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    
        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(userName, ct),
            Times.Once);

        _usersRepositoryMock.Verify(
            x => x.DeleteUserAsync(It.IsAny<Domain.Models.User>()),
            Times.Never);
    }
}