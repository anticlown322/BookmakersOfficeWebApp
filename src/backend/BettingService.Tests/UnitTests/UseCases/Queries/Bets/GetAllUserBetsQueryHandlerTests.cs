using AutoMapper;
using BettingService.BLL.DTO.Bet;
using BettingService.BLL.UseCases.Bets.Queries.GetAllUserBets;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.RequestFeatures;
using BettingService.DAL.RequestFeatures.Params;

namespace BettingService.Tests.UnitTests.UseCases.Queries.Bets;

public class GetAllUserBetsQueryHandlerTests
{
    private readonly Mock<IBetRepository> _betRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllUserBetsQueryHandler _handler;

    public GetAllUserBetsQueryHandlerTests()
    {
        _betRepositoryMock = new Mock<IBetRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new GetAllUserBetsQueryHandler(_betRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnPagedUserBets_WhenUserHasBets()
    {
        // Arrange
        var username = "testUser";
        var parameters = new BetParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllUserBetsQuery(parameters, username);
        var cancellationToken = CancellationToken.None;

        var userBets = new List<Bet>
        {
            new Bet { Id = Guid.NewGuid(), Username = username, Amount = 100 },
            new Bet { Id = Guid.NewGuid(), Username = username, Amount = 200 }
        };

        var pagedList = PagedList<Bet>.ToPagedList(
            userBets.AsQueryable(),
            parameters.PageNumber,
            parameters.PageSize);

        var expectedDtos = userBets.Select(b => new GetBetDto(
            b.Id, b.Username, "match1", b.Amount, 1.5m,
            BetStatus.Pending, DateTime.Now, null));

        _betRepositoryMock
            .Setup(x => x.GetUserBetsAsync(parameters, username, cancellationToken))
            .ReturnsAsync(pagedList);

        _mapperMock
            .Setup(x => x.Map<GetBetDto>(It.IsAny<Bet>()))
            .Returns<Bet>(b => expectedDtos.First(d => d.Id == b.Id));

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(userBets.Count);
        result.Data.All(dto => dto.Username == username).Should().BeTrue();
        result.MetaData.CurrentPage.Should().Be(parameters.PageNumber);
        result.MetaData.PageSize.Should().Be(parameters.PageSize);
        result.MetaData.TotalCount.Should().Be(userBets.Count);

        _betRepositoryMock.Verify(
            x => x.GetUserBetsAsync(parameters, username, cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyPagedResponse_WhenUserHasNoBets()
    {
        // Arrange
        var username = "userWithNoBets";
        var parameters = new BetParameters { PageNumber = 1, PageSize = 10 };
        var query = new GetAllUserBetsQuery(parameters, username);
        var cancellationToken = CancellationToken.None;

        var emptyPagedList = PagedList<Bet>.ToPagedList(
            new List<Bet>().AsQueryable(),
            parameters.PageNumber,
            parameters.PageSize);

        _betRepositoryMock
            .Setup(x => x.GetUserBetsAsync(parameters, username, cancellationToken))
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
            x => x.GetUserBetsAsync(parameters, username, cancellationToken),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<GetBetDto>(It.IsAny<Bet>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseMaxPageSize_WhenRequestedPageSizeExceedsMax()
    {
        // Arrange
        var username = "testUser";
        var parameters = new BetParameters { PageNumber = 1, PageSize = 100 };
        var query = new GetAllUserBetsQuery(parameters, username);
        var cancellationToken = CancellationToken.None;

        var expectedPageSize = 50;
        var userBets = BetsUseCasesTestData.CreateTestUserBets(username, 60);

        var pagedList = PagedList<Bet>.ToPagedList(
            userBets.AsQueryable(),
            parameters.PageNumber,
            expectedPageSize);

        _betRepositoryMock
            .Setup(x => x.GetUserBetsAsync(
                It.Is<BetParameters>(p => p.PageSize == expectedPageSize),
                username,
                cancellationToken))
            .ReturnsAsync(pagedList);

        // Act
        var result = await _handler.Handle(query, cancellationToken);

        // Assert
        result.MetaData.PageSize.Should().Be(expectedPageSize);
        _betRepositoryMock.Verify(
            x => x.GetUserBetsAsync(
                It.Is<BetParameters>(p => p.PageSize == expectedPageSize),
                username,
                cancellationToken),
            Times.Once);
    }
}