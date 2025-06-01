using AutoMapper;
using BettingService.BLL.DTO.Payout;
using BettingService.BLL.UseCases.Payouts.Queries.GetAllPayouts;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.Tests.UnitTests.UseCases.Queries.Payouts;

public class GetAllPayoutsQueryHandlerTests
{
    private readonly Mock<IPayoutRepository> _payoutRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllPayoutsQueryHandler _handler;

    public GetAllPayoutsQueryHandlerTests()
    {
        _payoutRepositoryMock = new Mock<IPayoutRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllPayoutsQueryHandler(_payoutRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResponseWithMappedPayouts()
    {
        // Arrange
        var parameters = new PayoutParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllPayoutsQuery(parameters);
        var cancellationToken = CancellationToken.None;

        var pagedPayouts = PayoutsUseCasesTestData.GetTestPagedPayouts(parameters);
        var testPayouts = pagedPayouts.ToList();

        var expectedDtos = testPayouts.Select(p => new GetPayoutDto(
            p.Id,
            p.BetId,
            p.Amount,
            p.Status,
            p.ProcessedAt.Value,
            p.ErrorReason)).ToList();

        _payoutRepositoryMock
            .Setup(r => r.GetAllPayoutsAsync(parameters, cancellationToken))
            .ReturnsAsync(pagedPayouts);

        _mapperMock
            .Setup(m => m.Map<GetPayoutDto>(It.IsAny<Payout>()))
            .Returns<Payout>(p => new GetPayoutDto(
                p.Id,
                p.BetId,
                p.Amount,
                p.Status,
                p.ProcessedAt.Value,
                p.ErrorReason));

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEquivalentTo(expectedDtos);
        result.MetaData.Should().NotBeNull();
        result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
        result.MetaData.PageSize.Should().Be(parameters.PageSize);
        result.MetaData.TotalCount.Should().Be(testPayouts.Count);

        _payoutRepositoryMock.Verify(
            r => r.GetAllPayoutsAsync(parameters, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            m => m.Map<GetPayoutDto>(It.IsAny<Payout>()),
            Times.Exactly(testPayouts.Count));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResponse_WhenNoPayoutsExist()
    {
        // Arrange
        var parameters = new PayoutParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllPayoutsQuery(parameters);
        var cancellationToken = CancellationToken.None;

        var emptyPayouts = new List<Payout>();
        var emptyPagedPayouts = PagedList<Payout>.ToPagedList(
            emptyPayouts.AsQueryable(),
            parameters.PageNumber,
            parameters.PageSize);

        _payoutRepositoryMock
            .Setup(r => r.GetAllPayoutsAsync(parameters, cancellationToken))
            .ReturnsAsync(emptyPagedPayouts);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.MetaData.Should().NotBeNull();
        result.MetaData.TotalCount.Should().Be(0);

        _payoutRepositoryMock.Verify(
            r => r.GetAllPayoutsAsync(parameters, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            m => m.Map<GetPayoutDto>(It.IsAny<Payout>()),
            Times.Never);
    }
}