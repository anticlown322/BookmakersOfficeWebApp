using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Payouts.Queries.GetPayoutById;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;

namespace BettingService.Tests.UnitTests.UseCases.Queries.Payouts;

using FluentAssertions;
using Moq;
using Xunit;

public class GetPayoutByIdQueryHandlerTests
{
    private readonly Mock<IPayoutRepository> _payoutRepositoryMock;
    private readonly GetPayoutByIdQueryHandler _handler;

    public GetPayoutByIdQueryHandlerTests()
    {
        _payoutRepositoryMock = new Mock<IPayoutRepository>();
        _handler = new GetPayoutByIdQueryHandler(_payoutRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPayout_WhenPayoutExists()
    {
        // Arrange
        var existingPayout = PayoutsUseCasesTestData.GetTestPayouts().First();
        var query = new GetPayoutByIdQuery(existingPayout.Id);
        var cancellationToken = CancellationToken.None;

        _payoutRepositoryMock
            .Setup(r => r.GetByIdAsync(existingPayout.Id, cancellationToken))
            .ReturnsAsync(existingPayout);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(existingPayout);
        _payoutRepositoryMock.Verify(
            r => r.GetByIdAsync(existingPayout.Id, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowPayoutNotFoundByIdException_WhenPayoutDoesNotExist()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();
        var query = new GetPayoutByIdQuery(nonExistingId);
        var cancellationToken = CancellationToken.None;

        _payoutRepositoryMock
            .Setup(r => r.GetByIdAsync(nonExistingId, cancellationToken))
            .ReturnsAsync((Payout?)null);

        // Act
        Func<Task> act = () => _handler.Handle(query, cancellationToken);

        // Assert
        await act.Should().ThrowAsync<PayoutNotFoundByIdException>()
            .WithMessage($"The payout with id: {nonExistingId} does not exist in the database.");
        _payoutRepositoryMock.Verify(
            r => r.GetByIdAsync(nonExistingId, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectIdToRepository()
    {
        // Arrange
        var testPayout = PayoutsUseCasesTestData.GetTestPayouts().First();
        var query = new GetPayoutByIdQuery(testPayout.Id);
        var cancellationToken = CancellationToken.None;

        _payoutRepositoryMock
            .Setup(r => r.GetByIdAsync(testPayout.Id, cancellationToken))
            .ReturnsAsync(testPayout);

        // Act
        await _handler.Handle(query, cancellationToken);

        // Assert
        _payoutRepositoryMock.Verify(
            r => r.GetByIdAsync(
                It.Is<Guid>(id => id == testPayout.Id),
                cancellationToken),
            Times.Once);
    }
}