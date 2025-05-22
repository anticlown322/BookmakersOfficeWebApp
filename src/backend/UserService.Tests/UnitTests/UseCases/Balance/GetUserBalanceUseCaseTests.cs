using UserService.Application.UseCases.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;
using UserService.Tests.UnitTests.UseCases.User;

namespace UserService.Tests.UnitTests.UseCases.Balance;

public class GetUserBalanceUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly GetUserBalanceUseCase _getUserBalanceUseCase;

    public GetUserBalanceUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _getUserBalanceUseCase = new GetUserBalanceUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_UserWithBalanceExists_ReturnsBalanceDto()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var expectedBalance = user.Balance.CurrentAmount;
        var expectedLastUpdated = user.Balance.LastUpdated;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act
        var result = await _getUserBalanceUseCase.ExecuteAsync(username, ct);

        // Assert
        result.CurrentBalance.Should().Be(expectedBalance);
        result.LastUpdated.Should().Be(expectedLastUpdated);

        _usersRepositoryMock.Verify(
            x => x.GetUserByNameAsync(username, ct),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotExists_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _getUserBalanceUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_BalanceDataIsNull_ThrowsBalanceDataIsNotFoundException()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        user.Balance = null;
        var username = user.UserName;
        var ct = CancellationToken.None;

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _getUserBalanceUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should()
            .ThrowAsync<BalanceDataIsNotFoundException>()
            .WithMessage($"Balance data is incorrect or balance can not be found for user {username}. ");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = UseCasesTestData.ValidUser.UserName;
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getUserBalanceUseCase.ExecuteAsync(username, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var username = UseCasesTestData.ValidUser.UserName;
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getUserBalanceUseCase.ExecuteAsync(username, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();
        exception.Which.Message.Should().Be(expectedException.Message);
    }
}