using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.UseCases.MatchResult;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.MatchResult;

public class GetMatchResultByIdUseCaseTests
{
    private readonly Mock<IMatchResultRepository> _matchResultRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetMatchResultByIdUseCase _getMatchResultByIdUseCase;

    public GetMatchResultByIdUseCaseTests()
    {
        _matchResultRepositoryMock = new Mock<IMatchResultRepository>();
        _mapperMock = new Mock<IMapper>();
        _getMatchResultByIdUseCase =
            new GetMatchResultByIdUseCase(_matchResultRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidId_ReturnsMappedMatchResult()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var testMatchResult = MatchResultUseCasesTestData.CreateTestMatchResultsWithMetadata(1).First();
        var testMatchResultDto = MatchResultUseCasesTestData.CreateTestMatchResultDtos(1).First();

        _matchResultRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testMatchResult);

        _mapperMock
            .Setup(x => x.Map<MatchResultGetDto>(testMatchResult))
            .Returns(testMatchResultDto);

        // Act
        var result = await _getMatchResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        result.Should().BeEquivalentTo(testMatchResultDto);

        _matchResultRepositoryMock.Verify(
            x => x.GetByIdAsync(validId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(testMatchResult),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidIdFormat_ThrowsInvalidIdFormatException()
    {
        // Arrange
        var invalidId = "invalid-id";
        var ct = CancellationToken.None;

        // Act
        Func<Task> act = () => _getMatchResultByIdUseCase.ExecuteAsync(invalidId, ct);

        // Assert
        await act.Should().ThrowAsync<InvalidIdFormatException>();

        _matchResultRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(It.IsAny<Domain.Models.Results.MatchResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentId_ThrowsMatchResultNotFoundByIdException()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439012";
        var ct = CancellationToken.None;

        _matchResultRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentId, ct))
            .ReturnsAsync((Domain.Models.Results.MatchResult)null);

        // Act
        Func<Task> act = () => _getMatchResultByIdUseCase.ExecuteAsync(nonExistentId, ct);

        // Assert
        await act.Should().ThrowAsync<MatchResultNotFoundByIdException>();

        _matchResultRepositoryMock.Verify(
            x => x.GetByIdAsync(nonExistentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(It.IsAny<Domain.Models.Results.MatchResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getMatchResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _matchResultRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MatchResultGetDto>(It.IsAny<Domain.Models.Results.MatchResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _matchResultRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getMatchResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var testMatchResult = MatchResultUseCasesTestData.CreateTestMatchResultsWithMetadata(1).First()
            ;
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _matchResultRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testMatchResult);

        _mapperMock
            .Setup(x => x.Map<MatchResultGetDto>(testMatchResult))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getMatchResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}