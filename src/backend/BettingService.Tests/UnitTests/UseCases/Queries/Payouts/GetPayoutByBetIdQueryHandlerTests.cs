using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Payouts.Queries.GetPayoutByBetId;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;

namespace BettingService.Tests.UnitTests.UseCases.Queries.Payouts;

using FluentAssertions;
using Moq;
using Xunit;

public class GetPayoutByBetIdQueryHandlerTests
{
    private readonly Mock<IPayoutRepository> _payoutRepositoryMock;
    private readonly Mock<IBetRepository> _betRepositoryMock;
    private readonly GetPayoutByBetIdQueryHandler _handler;

    public GetPayoutByBetIdQueryHandlerTests()
    {
        _payoutRepositoryMock = new Mock<IPayoutRepository>();
        _betRepositoryMock = new Mock<IBetRepository>();
        _handler = new GetPayoutByBetIdQueryHandler(_payoutRepositoryMock.Object, _betRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPayout_WhenBetAndPayoutExist()
    {
        // Arrange
        var testPayout = PayoutsUseCasesTestData.GetTestPayouts().First();
        var testBet = new Bet { Id = testPayout.BetId };
        var query = new GetPayoutByBetIdQuery(testPayout.BetId);
        var cancellationToken = CancellationToken.None;

        _betRepositoryMock
            .Setup(r => r.GetByIdAsync(testPayout.BetId, cancellationToken))
            .ReturnsAsync(testBet);

        _payoutRepositoryMock
            .Setup(r => r.GetByBetIdAsync(testPayout.BetId, cancellationToken))
            .ReturnsAsync(testPayout);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testPayout);
        
        _betRepositoryMock.Verify(
            r => r.GetByIdAsync(testPayout.BetId, cancellationToken),
            Times.Once);
        
        _payoutRepositoryMock.Verify(
            r => r.GetByBetIdAsync(testPayout.BetId, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowBetNotFoundByIdException_WhenBetDoesNotExist()
    {
        // Arrange
        var nonExistingBetId = Guid.NewGuid();
        var query = new GetPayoutByBetIdQuery(nonExistingBetId);
        var cancellationToken = CancellationToken.None;

        _betRepositoryMock
            .Setup(r => r.GetByIdAsync(nonExistingBetId, cancellationToken))
            .ReturnsAsync((Bet?)null);

        // Act
        Func<Task> act = () => _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<BetNotFoundByIdException>()
            .WithMessage($"The bet with id: {nonExistingBetId} does not exist in the database.");
        
        _betRepositoryMock.Verify(
            r => r.GetByIdAsync(nonExistingBetId, cancellationToken),
            Times.Once);
        
        _payoutRepositoryMock.Verify(
            r => r.GetByBetIdAsync(It.IsAny<Guid>(), cancellationToken),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldThrowPayoutNotFoundByBetIdException_WhenPayoutDoesNotExist()
    {
        // Arrange
        var testBet = new Bet { Id = Guid.NewGuid() };
        var query = new GetPayoutByBetIdQuery(testBet.Id);
        var cancellationToken = CancellationToken.None;

        _betRepositoryMock
            .Setup(r => r.GetByIdAsync(testBet.Id, cancellationToken))
            .ReturnsAsync(testBet);

        _payoutRepositoryMock
            .Setup(r => r.GetByBetIdAsync(testBet.Id, cancellationToken))
            .ReturnsAsync((Payout?)null);

        // Act
        Func<Task> act = () => _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<PayoutNotFoundByBetIdException>()
            .WithMessage($"The payout with bet id: {testBet.Id} does not exist in the database.");
        
        _betRepositoryMock.Verify(
            r => r.GetByIdAsync(testBet.Id, cancellationToken),
            Times.Once);
        
        _payoutRepositoryMock.Verify(
            r => r.GetByBetIdAsync(testBet.Id, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectBetIdToRepositories()
    {
        // Arrange
        var testPayout = PayoutsUseCasesTestData.GetTestPayouts().First();
        var testBet = new Bet { Id = testPayout.BetId };
        var query = new GetPayoutByBetIdQuery(testPayout.BetId);
        var cancellationToken = CancellationToken.None;

        _betRepositoryMock
            .Setup(r => r.GetByIdAsync(testPayout.BetId, cancellationToken))
            .ReturnsAsync(testBet);

        _payoutRepositoryMock
            .Setup(r => r.GetByBetIdAsync(testPayout.BetId, cancellationToken))
            .ReturnsAsync(testPayout);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _betRepositoryMock.Verify(
            r => r.GetByIdAsync(
                It.Is<Guid>(id => id == testPayout.BetId),
                cancellationToken),
            Times.Once);
        
        _payoutRepositoryMock.Verify(
            r => r.GetByBetIdAsync(
                It.Is<Guid>(id => id == testPayout.BetId),
                cancellationToken),
            Times.Once);
    }
}