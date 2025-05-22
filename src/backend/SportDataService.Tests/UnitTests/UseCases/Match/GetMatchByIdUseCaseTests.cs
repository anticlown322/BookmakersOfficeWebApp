using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.UseCases.Match;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.Match;

public class GetMatchByIdUseCaseTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetMatchByIdUseCase _getMatchByIdUseCase;

    public GetMatchByIdUseCaseTests()
    {
        _matchRepositoryMock = new Mock<IMatchRepository>();
        _mapperMock = new Mock<IMapper>();
        _getMatchByIdUseCase = new GetMatchByIdUseCase(_matchRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidId_ReturnsMappedMatch()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var testMatch = MatchUseCasesTestData.CreateTestMatchesWithMetadata(1).First();
        var testMatchDto = MatchUseCasesTestData.CreateTestMatchDtos(1).First();

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testMatch);

        _mapperMock
            .Setup(x => x.Map<MatchGetDto>(testMatch))
            .Returns(testMatchDto);

        // Act
        var result = await _getMatchByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        result.Should().BeEquivalentTo(testMatchDto);

        _matchRepositoryMock.Verify(
            x => x.GetByIdAsync(validId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(testMatch),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidIdFormat_ThrowsInvalidIdFormatException()
    {
        // Arrange
        var invalidId = "invalid-id";
        var ct = CancellationToken.None;

        // Act
        Func<Task> act = () => _getMatchByIdUseCase.ExecuteAsync(invalidId, ct);

        // Assert
        await act.Should().ThrowAsync<InvalidIdFormatException>();

        _matchRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(It.IsAny<Domain.Models.Prematch.Match>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentId_ThrowsMatchNotFoundByIdException()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439012";
        var ct = CancellationToken.None;

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentId, ct))
            .ReturnsAsync((Domain.Models.Prematch.Match)null);

        // Act
        Func<Task> act = () => _getMatchByIdUseCase.ExecuteAsync(nonExistentId, ct);

        // Assert
        await act.Should().ThrowAsync<MatchNotFoundByIdException>();

        _matchRepositoryMock.Verify(
            x => x.GetByIdAsync(nonExistentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(It.IsAny<Domain.Models.Prematch.Match>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getMatchByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _matchRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(It.IsAny<Domain.Models.Prematch.Match>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getMatchByIdUseCase.ExecuteAsync(validId, ct);

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
        var testMatch = MatchUseCasesTestData.CreateTestMatchesWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _matchRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testMatch);

        _mapperMock
            .Setup(x => x.Map<MatchGetDto>(testMatch))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getMatchByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}