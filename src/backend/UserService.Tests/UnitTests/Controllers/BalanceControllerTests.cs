using System.Text.Json;
using UserService.Application.Contracts.UseCases.Balance;
using UserService.Application.DTO.Balance;
using UserService.Domain.RequestFeatures;
using UserService.Presentation.Controllers;

namespace UserService.Tests.UnitTests.Controllers;

public class BalanceControllerTests
{
    private readonly Mock<IGetUserBalanceUseCase> _getUserBalanceUseCaseMock = new();
    private readonly Mock<IDepositToUserBalanceUseCase> _depositToUserBalanceUseCaseMock = new();
    private readonly Mock<IWithdrawFromUserBalanceUseCase> _withdrawFromUserBalanceUseCaseMock = new();
    private readonly Mock<IGetTransactionHistory> _getTransactionHistoryMock = new();
    private readonly BalanceController _controller;

    public BalanceControllerTests()
    {
        _controller = new BalanceController(
            _getUserBalanceUseCaseMock.Object,
            _depositToUserBalanceUseCaseMock.Object,
            _withdrawFromUserBalanceUseCaseMock.Object,
            _getTransactionHistoryMock.Object);

        var httpContext = new DefaultHttpContext();
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetCurrentBalance_ShouldReturnOkWithBalance_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var expectedBalance = new UserBalanceGetDto(100.50m, DateTime.UtcNow);
        _getUserBalanceUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedBalance);

        // Act
        var result = await _controller.GetCurrentBalance(username, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(expectedBalance);
    }

    [Fact]
    public async Task Deposit_ShouldCallUseCaseWithCorrectParameters()
    {
        // Arrange
        const string username = "testuser";
        var depositDto = new DepositRequestDto(50.25m, "Test deposit");
        var cancellationToken = new CancellationToken();
        _depositToUserBalanceUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<DepositRequestDto>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Deposit(username, depositDto, cancellationToken);

        // Assert
        _depositToUserBalanceUseCaseMock.Verify(x =>
            x.ExecuteAsync(username, depositDto, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Withdraw_ShouldCallUseCaseWithCorrectParameters()
    {
        // Arrange
        const string username = "testuser";
        var withdrawDto = new WithdrawRequestDto(30.75m, "123456", "Test withdraw");
        var cancellationToken = new CancellationToken();
        _withdrawFromUserBalanceUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<WithdrawRequestDto>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Withdraw(username, withdrawDto, cancellationToken);

        // Assert
        _withdrawFromUserBalanceUseCaseMock.Verify(x =>
            x.ExecuteAsync(username, withdrawDto, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetTransactionHistory_ShouldReturnOkWithPagedResult_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var transactionParameters = new TransactionParameters { PageNumber = 1, PageSize = 10 };

        var transactions = new List<TransactionDto>
        {
            new()
            {
                TransactionId = Guid.NewGuid().ToString(), Amount = 50.00m, OperationType = "Deposit",
                Status = "Completed", CreatedAt = DateTime.UtcNow
            },
            new()
            {
                TransactionId = Guid.NewGuid().ToString(), Amount = 20.00m, OperationType = "Withdraw",
                Status = "Completed", CreatedAt = DateTime.UtcNow
            }
        };

        var metaData = new MetaData
        {
            CurrentPage = 1,
            TotalPages = 1,
            PageSize = 10,
            TotalCount = 2
        };

        _getTransactionHistoryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<TransactionParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((transactions, metaData));

        // Act
        var result = await _controller.GetTransactionHistory(username, transactionParameters, CancellationToken.None);

        // Assert
        result.Should().BeOfType<OkObjectResult>()
            .Which.Value.Should().BeEquivalentTo(transactions);

        _controller.HttpContext.Response.Headers.Should().ContainKey("X-Pagination");
        var paginationHeader = _controller.HttpContext.Response.Headers["X-Pagination"].First();
        var deserializedMetaData = JsonSerializer.Deserialize<MetaData>(paginationHeader);
        deserializedMetaData.Should().BeEquivalentTo(metaData);
    }

    [Fact]
    public async Task GetTransactionHistory_ShouldUseDefaultParameters_WhenNotProvided()
    {
        // Arrange
        const string username = "testuser";
        var defaultParameters = new TransactionParameters();

        _getTransactionHistoryMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<TransactionParameters>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<TransactionDto>(), new MetaData()));

        // Act
        await _controller.GetTransactionHistory(username, defaultParameters, CancellationToken.None);

        // Assert
        _getTransactionHistoryMock.Verify(x =>
                x.ExecuteAsync(
                    username,
                    It.Is<TransactionParameters>(p =>
                        p.PageNumber == defaultParameters.PageNumber &&
                        p.PageSize == defaultParameters.PageSize),
                    It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Deposit_ShouldCallUseCaseAndReturnNoContent_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var depositDto = new DepositRequestDto(50.25m, "Test deposit");
        var wasCalled = false;
        _depositToUserBalanceUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<DepositRequestDto>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => wasCalled = true)
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Deposit(username, depositDto, CancellationToken.None);

        // Assert
        wasCalled.Should().BeTrue();
    }

    [Fact]
    public async Task Withdraw_ShouldCallUseCaseAndReturnNoContent_WhenSuccess()
    {
        // Arrange
        const string username = "testuser";
        var withdrawDto = new WithdrawRequestDto(30.75m, "123456", "Test withdraw");
        var wasCalled = false;
        _withdrawFromUserBalanceUseCaseMock
            .Setup(x => x.ExecuteAsync(It.IsAny<string>(), It.IsAny<WithdrawRequestDto>(),
                It.IsAny<CancellationToken>()))
            .Callback(() => wasCalled = true)
            .Returns(Task.CompletedTask);

        // Act
        await _controller.Withdraw(username, withdrawDto, CancellationToken.None);

        // Assert
        wasCalled.Should().BeTrue();
    }
}