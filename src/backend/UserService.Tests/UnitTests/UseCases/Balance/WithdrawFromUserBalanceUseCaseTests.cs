using System.Collections;
using UserService.Application.DTO.Balance;
using UserService.Application.UseCases.Balance;
using UserService.Application.Utility;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Balance;

public class WithdrawFromUserBalanceUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly WithdrawFromUserBalanceUseCase _withdrawFromUserBalanceUseCase;

    public WithdrawFromUserBalanceUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _withdrawFromUserBalanceUseCase = new WithdrawFromUserBalanceUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidWithdraw_UpdatesUserBalance()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithBalance(1000m);
        var username = user.UserName;
        var ct = CancellationToken.None;
        var initialBalance = user.Balance.CurrentAmount;
        var withdrawAmount = 500m;
        var withdrawRequest = new WithdrawRequestDto(withdrawAmount, "Test withdraw");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        await _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        user.Balance.CurrentAmount.Should().Be(initialBalance - withdrawAmount);
        user.Balance.Transactions.Count.Should().Be(1);
        user.Balance.Transactions.First().Amount.Should().Be(withdrawAmount);
        user.Balance.Transactions.First().OperationType.Should()
            .BeEquivalentTo(BalanceOperationTypesAndStatuses.WithdrawOperation);
        
        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.UpdateUserAsync(user, ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;
        var withdrawRequest = new WithdrawRequestDto(500m, "Test withdraw");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");

    }

    [Fact]
    public async Task ExecuteAsync_BalanceIsNull_ThrowsBalanceDataIsNotFoundException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithoutBalance();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var withdrawRequest = new WithdrawRequestDto(500m, "Test withdraw");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        await act.Should()
            .ThrowAsync<BalanceDataIsNotFoundException>()
            .WithMessage($"Balance data is incorrect or balance can not be found for user {username}. ");

    }

    [Fact]
    public async Task ExecuteAsync_InsufficientFunds_ThrowsInvalidBalanceWithdrawException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithBalance(100m);
        var username = user.UserName;
        var ct = CancellationToken.None;
        var withdrawRequest = new WithdrawRequestDto(500m, "Test withdraw");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        await act.Should().ThrowAsync<InvalidBalanceWithdrawException>();

    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedBeforeUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = "testUser";
        var ct = new CancellationToken(canceled: true);
        var withdrawRequest = new WithdrawRequestDto(500m, "Test withdraw");

        // Act
        Func<Task> act = () => _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedAfterUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithBalance(1000m);
        var username = user.UserName;
        var cts = new CancellationTokenSource();
        var withdrawRequest = new WithdrawRequestDto(500m, "Test withdraw");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Callback(() => cts.Cancel());

        // Act
        Func<Task> act = () => _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithBalance(1000m);
        var username = user.UserName;
        var ct = CancellationToken.None;
        var withdrawRequest = new WithdrawRequestDto(500m, "Test withdraw");
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);

    }

    [Fact]
    public async Task ExecuteAsync_TransactionAddedCorrectly()
    {
        // Arrange
        var user = UseCasesTestData.CreateUserWithBalance(1000m);
        user.Balance.Transactions = new List<BalanceTransaction>();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var withdrawAmount = 500m;
        var withdrawCode = "abc123";
        var withdrawComment = "Test withdraw";
        var withdrawRequest = new WithdrawRequestDto(withdrawAmount, withdrawCode, withdrawComment);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        await _withdrawFromUserBalanceUseCase.ExecuteAsync(username, withdrawRequest, ct);

        // Assert
        var transaction = user.Balance.Transactions.Single();
        transaction.Amount.Should().Be(withdrawAmount);
        transaction.Comment.Should().Be(withdrawComment);
        transaction.OperationType.Should().BeEquivalentTo(BalanceOperationTypesAndStatuses.WithdrawOperation);
        transaction.UserId.Should().Be(user.Id);
    }
}