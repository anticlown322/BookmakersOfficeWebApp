using AutoMapper;
using BettingService.BLL.DTO.Bet;
using BettingService.BLL.UseCases.Bets.Queries.GetAllBets;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;
using FluentAssertions;
using Moq;

namespace BettingService.Tests.UnitTests.UseCases.Queries.Bets;

public class GetAllBetsQueryHandlerTests
{
    private readonly Mock<IBetRepository> _betRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllBetsQueryHandler _handler;

    public GetAllBetsQueryHandlerTests()
    {
        _betRepositoryMock = new Mock<IBetRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllBetsQueryHandler(_betRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedResponseWithBets_WhenBetsExist()
    {
        // Arrange
        var parameters = new BetParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllBetsQuery(parameters);
        var cancellationToken = CancellationToken.None;

        var bets = new List<Bet>
        {
            new Bet { Id = Guid.NewGuid(), Amount = 100, Odds = 1.5m },
            new Bet { Id = Guid.NewGuid(), Amount = 200, Odds = 2.0m }
        };

        var pagedList = PagedList<Bet>.ToPagedList(
            bets.AsQueryable(), 
            parameters.PageNumber, 
            parameters.PageSize);

        var expectedDtos = bets.Select(b => new GetBetDto(
            b.Id, "testUser", "match1", b.Amount, b.Odds, 
            BetStatus.Pending, DateTime.Now, null));

        _betRepositoryMock
            .Setup(x => x.GetAllBetsAsync(parameters, cancellationToken))
            .ReturnsAsync(pagedList);

        _mapperMock
            .Setup(x => x.Map<GetBetDto>(It.IsAny<Bet>()))
            .Returns<Bet>(b => expectedDtos.First(d => d.Id == b.Id));

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(bets.Count);
        result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
        result.MetaData.PageSize.Should().Be(parameters.PageSize);
        result.MetaData.TotalCount.Should().Be(bets.Count);

        _betRepositoryMock.Verify(
            x => x.GetAllBetsAsync(parameters, cancellationToken), 
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<GetBetDto>(It.IsAny<Bet>()), 
            Times.Exactly(bets.Count));
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResponse_WhenNoBetsExist()
    {
        // Arrange
        var parameters = new BetParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllBetsQuery(parameters);
        var cancellationToken = CancellationToken.None;

        var emptyBets = new List<Bet>();
        var emptyPagedList = PagedList<Bet>.ToPagedList(
            emptyBets.AsQueryable(), 
            parameters.PageNumber, 
            parameters.PageSize);

        _betRepositoryMock
            .Setup(x => x.GetAllBetsAsync(parameters, cancellationToken))
            .ReturnsAsync(emptyPagedList);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().BeEmpty();
        result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
        result.MetaData.PageSize.Should().Be(parameters.PageSize);
        result.MetaData.TotalCount.Should().Be(0);

        _betRepositoryMock.Verify(
            x => x.GetAllBetsAsync(parameters, cancellationToken), 
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<GetBetDto>(It.IsAny<Bet>()), 
            Times.Never);
    }
}