using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.UseCases.Match;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Tests.UnitTests.UseCases.Match;

public class GetAllMatchesUseCaseTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllMatchesUseCase _getAllMatchesUseCase;

    public GetAllMatchesUseCaseTests()
    {
        _matchRepositoryMock = new Mock<IMatchRepository>();
        _mapperMock = new Mock<IMapper>();
        _getAllMatchesUseCase = new GetAllMatchesUseCase(_matchRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedMatchesAndMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var matchParams = new MatchParameters();
        var testMatches = UseCasesTestData.CreateTestMatchesWithMetadata(3);
        var testMatchDtos = UseCasesTestData.CreateTestMatchDtos(3);

        _matchRepositoryMock
            .Setup(x => x.FindAllMatchesAsync(matchParams, ct))
            .ReturnsAsync(testMatches);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MatchGetDto>>(testMatches))
            .Returns(testMatchDtos);

        // Act
        var result = await _getAllMatchesUseCase.ExecuteAsync(matchParams, ct);

        // Assert
        result.matches.Should().BeEquivalentTo(testMatchDtos);
        result.metaData.Should().Be(testMatches.MetaData);

        _matchRepositoryMock.Verify(
            x => x.FindAllMatchesAsync(matchParams, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<MatchGetDto>>(testMatches),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyResult_ReturnsEmptyCollectionWithMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var matchParams = new MatchParameters();
        var emptyMatches = UseCasesTestData.CreateTestMatchesWithMetadata(0);
        var emptyDtos = UseCasesTestData.CreateTestMatchDtos(0);

        _matchRepositoryMock
            .Setup(x => x.FindAllMatchesAsync(matchParams, ct))
            .ReturnsAsync(emptyMatches);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MatchGetDto>>(emptyMatches))
            .Returns(emptyDtos);

        // Act
        var result = await _getAllMatchesUseCase.ExecuteAsync(matchParams, ct);

        // Assert
        result.matches.Should().BeEmpty();
        result.metaData.Should().Be(emptyMatches.MetaData);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);
        var matchParams = new MatchParameters();

        // Act
        Func<Task> act = () => _getAllMatchesUseCase.ExecuteAsync(matchParams, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _matchRepositoryMock.Verify(
            x => x.FindAllMatchesAsync(It.IsAny<MatchParameters>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var matchParams = new MatchParameters();
        var expectedException = new Exception("Database error");

        _matchRepositoryMock
            .Setup(x => x.FindAllMatchesAsync(matchParams, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getAllMatchesUseCase.ExecuteAsync(matchParams, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var matchParams = new MatchParameters();
        var testMatches = UseCasesTestData.CreateTestMatchesWithMetadata(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _matchRepositoryMock
            .Setup(x => x.FindAllMatchesAsync(matchParams, ct))
            .ReturnsAsync(testMatches);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MatchGetDto>>(testMatches))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getAllMatchesUseCase.ExecuteAsync(matchParams, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}