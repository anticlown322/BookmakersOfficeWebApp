using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.UseCases.MatchResult;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Tests.UnitTests.UseCases.MatchResult;

public class GetAllMatchResultsUseCaseTests
{
    private readonly Mock<IMatchResultRepository> _matchResultRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllMatchResultsUseCase _getAllMatchResultsUseCase;

    public GetAllMatchResultsUseCaseTests()
    {
        _matchResultRepositoryMock = new Mock<IMatchResultRepository>();
        _mapperMock = new Mock<IMapper>();
        _getAllMatchResultsUseCase = new GetAllMatchResultsUseCase(_matchResultRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedResultsAndMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new MatchResultParameters();
        var testResults = UseCasesTestData.CreateTestMatchResultsWithMetadata(3);
        var testResultDtos = UseCasesTestData.CreateTestMatchResultDtos(3);

        _matchResultRepositoryMock
            .Setup(x => x.FindAllMatchResultsAsync(parameters, ct))
            .ReturnsAsync(testResults);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MatchResultGetDto>>(testResults))
            .Returns(testResultDtos);

        // Act
        var result = await _getAllMatchResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.matchResults.Should().BeEquivalentTo(testResultDtos);
        result.metaData.Should().Be(testResults.MetaData);

        _matchResultRepositoryMock.Verify(
            x => x.FindAllMatchResultsAsync(parameters, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<MatchResultGetDto>>(testResults),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyResult_ReturnsEmptyCollectionWithMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new MatchResultParameters();
        var emptyResults = UseCasesTestData.CreateTestMatchResultsWithMetadata(0);
        var emptyDtos = UseCasesTestData.CreateTestMatchResultDtos(0);

        _matchResultRepositoryMock
            .Setup(x => x.FindAllMatchResultsAsync(parameters, ct))
            .ReturnsAsync(emptyResults);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MatchResultGetDto>>(emptyResults))
            .Returns(emptyDtos);

        // Act
        var result = await _getAllMatchResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.matchResults.Should().BeEmpty();
        result.metaData.Should().Be(emptyResults.MetaData);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);
        var parameters = new MatchResultParameters();

        // Act
        Func<Task> act = () => _getAllMatchResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _matchResultRepositoryMock.Verify(
            x => x.FindAllMatchResultsAsync(It.IsAny<MatchResultParameters>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new MatchResultParameters();
        var expectedException = new Exception("Database error");

        _matchResultRepositoryMock
            .Setup(x => x.FindAllMatchResultsAsync(parameters, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getAllMatchResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new MatchResultParameters();
        var testResults = UseCasesTestData.CreateTestMatchResultsWithMetadata(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _matchResultRepositoryMock
            .Setup(x => x.FindAllMatchResultsAsync(parameters, ct))
            .ReturnsAsync(testResults);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<MatchResultGetDto>>(testResults))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getAllMatchResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}