using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.UseCases.TournamentResult;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.TournamentResult;

public class GetTournamentResultByResultIdUseCaseTests
{
    private readonly Mock<ITournamentResultRepository> _tournamentResultRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTournamentResultByResultIdUseCase _getTournamentResultByResultIdUseCase;

    public GetTournamentResultByResultIdUseCaseTests()
    {
        _tournamentResultRepositoryMock = new Mock<ITournamentResultRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTournamentResultByResultIdUseCase = new GetTournamentResultByResultIdUseCase(
            _tournamentResultRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidTournamentResultId_ReturnsMappedTournamentResult()
    {
        // Arrange
        var validTournamentResultId = "10012345";
        var ct = CancellationToken.None;
        var testTournamentResult = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(1).First();
        var testTournamentResultDto = TournamentResultUseCasesTestData.CreateTestTournamentResultDtos(1).First();

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(validTournamentResultId, ct))
            .ReturnsAsync(testTournamentResult);

        _mapperMock
            .Setup(x => x.Map<TournamentResultGetDto>(testTournamentResult))
            .Returns(testTournamentResultDto);

        // Act
        var result = await _getTournamentResultByResultIdUseCase.ExecuteAsync(validTournamentResultId, ct);

        // Assert
        result.Should().BeEquivalentTo(testTournamentResultDto);

        _tournamentResultRepositoryMock.Verify(
            x => x.GetTournamentResultByTournamentResultIdAsync(validTournamentResultId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(testTournamentResult),
            Times.Once);
    }

    [Fact]
    public async Task
        ExecuteAsync_NonExistentTournamentResultId_ThrowsTournamentResultNotFoundByTournamentResultIdException()
    {
        // Arrange
        var nonExistentTournamentResultId = "10099999";
        var ct = CancellationToken.None;

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(nonExistentTournamentResultId, ct))
            .ReturnsAsync((Domain.Models.Results.TournamentResult)null);

        // Act
        Func<Task> act = () => _getTournamentResultByResultIdUseCase.ExecuteAsync(nonExistentTournamentResultId, ct);

        // Assert
        await act.Should().ThrowAsync<TournamentResultNotFoundByTournamentResultIdException>();

        _tournamentResultRepositoryMock.Verify(
            x => x.GetTournamentResultByTournamentResultIdAsync(nonExistentTournamentResultId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(It.IsAny<Domain.Models.Results.TournamentResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validTournamentResultId = "10012345";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getTournamentResultByResultIdUseCase.ExecuteAsync(validTournamentResultId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _tournamentResultRepositoryMock.Verify(
            x => x.GetTournamentResultByTournamentResultIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(It.IsAny<Domain.Models.Results.TournamentResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validTournamentResultId = "10012345";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(validTournamentResultId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTournamentResultByResultIdUseCase.ExecuteAsync(validTournamentResultId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var validTournamentResultId = "10012345";
        var ct = CancellationToken.None;
        var testTournamentResult = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(validTournamentResultId, ct))
            .ReturnsAsync(testTournamentResult);

        _mapperMock
            .Setup(x => x.Map<TournamentResultGetDto>(testTournamentResult))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTournamentResultByResultIdUseCase.ExecuteAsync(validTournamentResultId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}