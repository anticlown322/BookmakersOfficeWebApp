using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Bets.Queries.GetBetById;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;

namespace BettingService.Tests.UnitTests.UseCases.Queries.Bets;

public class GetBetByIdQueryHandlerTests
{
    private readonly Mock<IBetRepository> _betRepositoryMock;
    private readonly GetBetByIdQueryHandler _handler;

    public GetBetByIdQueryHandlerTests()
    {
        _betRepositoryMock = new Mock<IBetRepository>();
        _handler = new GetBetByIdQueryHandler(_betRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnBet_WhenBetExists()
    {
        // Arrange
        var existingBetId = Guid.NewGuid();
        var query = new GetBetByIdQuery(existingBetId);
        var cancellationToken = CancellationToken.None;

        var expectedBet = BetsUseCasesTestData.CreateTestBet(existingBetId);
        _betRepositoryMock
            .Setup(x => x.GetByIdAsync(existingBetId, cancellationToken))
            .ReturnsAsync(expectedBet);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedBet);
        _betRepositoryMock.Verify(
            x => x.GetByIdAsync(existingBetId, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBetNotFoundByIdException_WhenBetDoesNotExist()
    {
        // Arrange
        var nonExistingBetId = Guid.NewGuid();
        var query = new GetBetByIdQuery(nonExistingBetId);
        var cancellationToken = CancellationToken.None;

        _betRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistingBetId, cancellationToken))
            .ReturnsAsync((Bet)null);

        // Act
        Func<Task> act = () => _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<BetNotFoundByIdException>()
            .WithMessage($"The bet with id: {nonExistingBetId} does not exist in the database.");
        
        _betRepositoryMock.Verify(
            x => x.GetByIdAsync(nonExistingBetId, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectBet_WhenMultipleBetsExist()
    {
        // Arrange
        var targetBetId = Guid.NewGuid();
        var otherBetId = Guid.NewGuid();
        var query = new GetBetByIdQuery(targetBetId);
        var cancellationToken = CancellationToken.None;

        var targetBet = BetsUseCasesTestData.CreateTestBet(targetBetId);
        var otherBet = BetsUseCasesTestData.CreateTestBet(otherBetId);

        _betRepositoryMock
            .Setup(x => x.GetByIdAsync(targetBetId, cancellationToken))
            .ReturnsAsync(targetBet);

        _betRepositoryMock
            .Setup(x => x.GetByIdAsync(otherBetId, cancellationToken))
            .ReturnsAsync(otherBet);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(targetBetId);
        result.Should().NotBeSameAs(otherBet);
        _betRepositoryMock.Verify(
            x => x.GetByIdAsync(targetBetId, cancellationToken),
            Times.Once);
    }
}