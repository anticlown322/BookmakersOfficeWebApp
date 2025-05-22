using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.UseCases.MatchResult;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.MatchResult;

public class GetMatchResultByResultIdUseCaseTests
{
    private readonly Mock<IMatchResultRepository> _matchResultRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetMatchResultByResultIdUseCase _getMatchResultByResultIdUseCase;

    public GetMatchResultByResultIdUseCaseTests()
    {
        _matchResultRepositoryMock = new Mock<IMatchResultRepository>();
        _mapperMock = new Mock<IMapper>();
        _getMatchResultByResultIdUseCase = new GetMatchResultByResultIdUseCase(_matchResultRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidResultId_ReturnsMappedMatchResult()
    {
        // Arrange
        var validResultId = "50012345";
        var ct = CancellationToken.None;
        var testMatchResult = MatchResultUseCasesTestData.CreateTestMatchResultsWithMetadata(1).First();
        var testMatchResultDto = MatchResultUseCasesTestData.CreateTestMatchResultDtos(1).First();

        _matchResultRepositoryMock
            .Setup(x => x.GetMatchResultByMatchResultIdAsync(validResultId, ct))
            .ReturnsAsync(testMatchResult);

        _mapperMock
            .Setup(x => x.Map<MatchResultGetDto>(testMatchResult))
            .Returns(testMatchResultDto);

        // Act
        var result = await _getMatchResultByResultIdUseCase.ExecuteAsync(validResultId, ct);

        // Assert
        result.Should().BeEquivalentTo(testMatchResultDto);

        _matchResultRepositoryMock.Verify(
            x => x.GetMatchResultByMatchResultIdAsync(validResultId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(testMatchResult),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentResultId_ThrowsMatchResultNotFoundByMatchResultIdException()
    {
        // Arrange
        var nonExistentResultId = "50099999";
        var ct = CancellationToken.None;

        _matchResultRepositoryMock
            .Setup(x => x.GetMatchResultByMatchResultIdAsync(nonExistentResultId, ct))
            .ReturnsAsync((Domain.Models.Results.MatchResult)null);

        // Act
        Func<Task> act = () => _getMatchResultByResultIdUseCase.ExecuteAsync(nonExistentResultId, ct);

        // Assert
        await act.Should().ThrowAsync<MatchResultNotFoundByMatchResultIdException>();

        _matchResultRepositoryMock.Verify(
            x => x.GetMatchResultByMatchResultIdAsync(nonExistentResultId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(It.IsAny<Domain.Models.Results.MatchResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validResultId = "50012345";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getMatchResultByResultIdUseCase.ExecuteAsync(validResultId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _matchResultRepositoryMock.Verify(
            x => x.GetMatchResultByMatchResultIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(It.IsAny<Domain.Models.Results.MatchResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validResultId = "50012345";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _matchResultRepositoryMock
            .Setup(x => x.GetMatchResultByMatchResultIdAsync(validResultId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getMatchResultByResultIdUseCase.ExecuteAsync(validResultId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var validResultId = "50012345";
        var ct = CancellationToken.None;
        var testMatchResult = MatchResultUseCasesTestData.CreateTestMatchResultsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _matchResultRepositoryMock
            .Setup(x => x.GetMatchResultByMatchResultIdAsync(validResultId, ct))
            .ReturnsAsync(testMatchResult);

        _mapperMock
            .Setup(x => x.Map<MatchResultGetDto>(testMatchResult))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getMatchResultByResultIdUseCase.ExecuteAsync(validResultId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}