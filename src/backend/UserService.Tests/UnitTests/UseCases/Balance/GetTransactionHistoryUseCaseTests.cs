using AutoMapper;
using UserService.Application.DTO.Balance;
using UserService.Application.UseCases.Balance;
using UserService.Application.Validation.Exceptions.Specific;
using UserService.Domain.RepositoryContracts;
using UserService.Domain.RequestFeatures;

namespace UserService.Tests.UnitTests.UseCases.Balance;

public class GetTransactionHistoryUseCaseTests
{
    private readonly Mock<IUsersRepository> _usersRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTransactionHistoryUseCase _getTransactionHistoryUseCase;

    public GetTransactionHistoryUseCaseTests()
    {
        _usersRepositoryMock = new Mock<IUsersRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTransactionHistoryUseCase =
            new GetTransactionHistoryUseCase(_usersRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidRequest_ReturnsMappedTransactions()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var transactionParameters = new TransactionParameters();
        var transactions = UseCasesTestData.CreateTestTransactions(3);
        var transactionDtos = UseCasesTestData.CreateTestTransactionDtos(3);

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GetAllBalanceTransactionsAsync(transactionParameters, user, ct))
            .ReturnsAsync(transactions);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TransactionDto>>(transactions))
            .Returns(transactionDtos);

        // Act
        var result = await _getTransactionHistoryUseCase.ExecuteAsync(username, transactionParameters, ct);

        // Assert
        result.transactions.Should().BeEquivalentTo(transactionDtos);
        result.metaData.Should().BeEquivalentTo(transactions.MetaData);

        _usersRepositoryMock.Verify(x => x.GetUserByNameAsync(username, ct), Times.Once);
        _usersRepositoryMock.Verify(x => x.GetAllBalanceTransactionsAsync(transactionParameters, user, ct), Times.Once);
        _mapperMock.Verify(x => x.Map<IEnumerable<TransactionDto>>(transactions), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_UserNotFound_ThrowsUserNotFoundByNameException()
    {
        // Arrange
        var username = "nonExistingUser";
        var ct = CancellationToken.None;
        var transactionParameters = new TransactionParameters();

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync((Domain.Models.User?)null);

        // Act
        Func<Task> act = () => _getTransactionHistoryUseCase.ExecuteAsync(username, transactionParameters, ct);

        // Assert
        await act.Should()
            .ThrowAsync<UserNotFoundByNameException>()
            .WithMessage($"The user with name: {username} does not exist in the database.");
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequestedBeforeUserLookup_ThrowsOperationCanceledException()
    {
        // Arrange
        var username = UseCasesTestData.ValidUser.UserName;
        var ct = new CancellationToken(canceled: true);
        var transactionParameters = new TransactionParameters();

        // Act
        Func<Task> act = () => _getTransactionHistoryUseCase.ExecuteAsync(username, transactionParameters, ct);

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
        var transactionParameters = new TransactionParameters();

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user)
            .Callback(() => cts.Cancel());

        // Act
        Func<Task> act = () => _getTransactionHistoryUseCase.ExecuteAsync(username, transactionParameters, cts.Token);

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
        var transactionParameters = new TransactionParameters();
        var expectedException = new Exception("Database error");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTransactionHistoryUseCase.ExecuteAsync(username, transactionParameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);

    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var user = UseCasesTestData.ValidUser;
        var username = user.UserName;
        var ct = CancellationToken.None;
        var transactionParameters = new TransactionParameters();
        var transactions = UseCasesTestData.CreateTestTransactions(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _usersRepositoryMock
            .Setup(x => x.GetUserByNameAsync(username, ct))
            .ReturnsAsync(user);

        _usersRepositoryMock
            .Setup(x => x.GetAllBalanceTransactionsAsync(transactionParameters, user, ct))
            .ReturnsAsync(transactions);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TransactionDto>>(transactions))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTransactionHistoryUseCase.ExecuteAsync(username, transactionParameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);

    }
}