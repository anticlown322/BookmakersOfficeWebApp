using UserService.Application.DTO.Balance;
using UserService.Application.UseCases.Balance;
using UserService.Application.Utility;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;

namespace UserService.Tests.UnitTests.UseCases.Balance;

public class DepositToUserBalanceUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly DepositToUserBalanceUseCase _depositToUserBalanceUseCase;

    public DepositToUserBalanceUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _depositToUserBalanceUseCase = new DepositToUserBalanceUseCase(_usersRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidDeposit_UpdatesUserBalance()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var initialBalance = user.Balance.CurrentAmount;
        var depositAmount = 50.0m;
        var depositRequest = new DepositRequestDto(depositAmount, "Test deposit");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        await _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, ct);

        // Assert
        user.Balance.CurrentAmount.Should().Be(initialBalance + depositAmount);
        user.Balance.Transactions.Count.Should().Be(1);
        user.Balance.Transactions.First().Amount.Should().Be(depositAmount);
        user.Balance.Transactions.First().OperationType.Should().Be(BalanceOperationTypesAndStatuses.DepositOperation);

        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.UpdateUserAsync(user, ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;
        var depositRequest = new DepositRequestDto(50.0m, "Test deposit");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_BalanceIsNull_ThrowsBalanceDataIsNotFoundException()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        user.Balance = null;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var depositRequest = new DepositRequestDto(50.0m, "Test deposit");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        // Act
        Func<Task> act = () => _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, ct);

        // Assert
        await act.Should()
            .ThrowAsync<BalanceDataIsNotFoundException>()
            .WithMessage($"Balance data is incorrect or balance can not be found for user {username}. ");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedBeforeUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = UseCasesTestData.ValidUser.UserName;
        var ct = new CancellationToken(canceled: true);
        var depositRequest = new DepositRequestDto(50.0m, "Test deposit");

        // Act
        Func<Task> act = () => _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedAfterUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var cts = new CancellationTokenSource();
        var depositRequest = new DepositRequestDto(50.0m, "Test deposit");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Callback(() => cts.Cancel());

        // Act
        Func<Task> act = () => _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var depositRequest = new DepositRequestDto(50.0m, "Test deposit");
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_TransactionAddedCorrectly()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        user.Balance.Transactions = new List<BalanceTransaction>();
        var username = user.UserName;
        var ct = CancellationToken.None;
        var depositAmount = 100.0m;
        var depositComment = "Test transaction";
        var depositRequest = new DepositRequestDto(depositAmount, depositComment);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.UpdateUserAsync(user, ct))
            .Returns(Task.CompletedTask);

        // Act
        await _depositToUserBalanceUseCase.ExecuteAsync(username, depositRequest, ct);

        // Assert
        var transaction = user.Balance.Transactions.Single();
        transaction.Amount.Should().Be(depositAmount);
        transaction.Comment.Should().Be(depositComment);
        transaction.OperationType.Should().Be(BalanceOperationTypesAndStatuses.DepositOperation);
        transaction.UserId.Should().Be(user.Id);
    }
}