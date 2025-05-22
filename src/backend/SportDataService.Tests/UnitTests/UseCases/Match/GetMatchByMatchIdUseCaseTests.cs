using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.UseCases.Match;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.Match;

public class GetMatchByMatchIdUseCaseTests
{
    private readonly Mock<IMatchRepository> _matchRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetMatchByMatchIdUseCase _getMatchByMatchIdUseCase;

    public GetMatchByMatchIdUseCaseTests()
    {
        _matchRepositoryMock = new Mock<IMatchRepository>();
        _mapperMock = new Mock<IMapper>();
        _getMatchByMatchIdUseCase = new GetMatchByMatchIdUseCase(_matchRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidMatchId_ReturnsMappedMatch()
    {
        // Arrange
        var validMatchId = "50012345";
        var ct = CancellationToken.None;
        var testMatch = UseCasesTestData.CreateTestMatchesWithMetadata(1).First();
        var testMatchDto = UseCasesTestData.CreateTestMatchDtos(1).First();

        _matchRepositoryMock
            .Setup(x => x.GetMatchByMatchIdAsync(validMatchId, ct))
            .ReturnsAsync(testMatch);

        _mapperMock
            .Setup(x => x.Map<MatchGetDto>(testMatch))
            .Returns(testMatchDto);

        // Act
        var result = await _getMatchByMatchIdUseCase.ExecuteAsync(validMatchId, ct);

        // Assert
        result.Should().BeEquivalentTo(testMatchDto);

        _matchRepositoryMock.Verify(
            x => x.GetMatchByMatchIdAsync(validMatchId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(testMatch),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentMatchId_ThrowsMatchNotFoundByMatchIdException()
    {
        // Arrange
        var nonExistentMatchId = "50099999";
        var ct = CancellationToken.None;

        _matchRepositoryMock
            .Setup(x => x.GetMatchByMatchIdAsync(nonExistentMatchId, ct))
            .ReturnsAsync((Domain.Models.Prematch.Match)null);

        // Act
        Func<Task> act = () => _getMatchByMatchIdUseCase.ExecuteAsync(nonExistentMatchId, ct);

        // Assert
        await act.Should().ThrowAsync<MatchNotFoundByMatchIdException>();

        _matchRepositoryMock.Verify(
            x => x.GetMatchByMatchIdAsync(nonExistentMatchId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(It.IsAny<Domain.Models.Prematch.Match>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validMatchId = "50012345";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getMatchByMatchIdUseCase.ExecuteAsync(validMatchId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _matchRepositoryMock.Verify(
            x => x.GetMatchByMatchIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<MatchGetDto>(It.IsAny<Domain.Models.Prematch.Match>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validMatchId = "50012345";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _matchRepositoryMock
            .Setup(x => x.GetMatchByMatchIdAsync(validMatchId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getMatchByMatchIdUseCase.ExecuteAsync(validMatchId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var validMatchId = "50012345";
        var ct = CancellationToken.None;
        var testMatch = UseCasesTestData.CreateTestMatchesWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _matchRepositoryMock
            .Setup(x => x.GetMatchByMatchIdAsync(validMatchId, ct))
            .ReturnsAsync(testMatch);

        _mapperMock
            .Setup(x => x.Map<MatchGetDto>(testMatch))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getMatchByMatchIdUseCase.ExecuteAsync(validMatchId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}