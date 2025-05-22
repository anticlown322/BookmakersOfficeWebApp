using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.UseCases.TournamentResult;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.TournamentResult;

public class GetTournamentResultByIdUseCaseTests
{
    private readonly Mock<ITournamentResultRepository> _tournamentResultRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTournamentResultByIdUseCase _getTournamentResultByIdUseCase;

    public GetTournamentResultByIdUseCaseTests()
    {
        _tournamentResultRepositoryMock = new Mock<ITournamentResultRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTournamentResultByIdUseCase = new GetTournamentResultByIdUseCase(
            _tournamentResultRepositoryMock.Object, 
            _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidId_ReturnsMappedTournamentResult()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var testTournamentResult = UseCasesTestData.CreateTestTournamentResultsWithMetadata(1).First();
        var testTournamentResultDto = UseCasesTestData.CreateTestTournamentResultDtos(1).First();

        _tournamentResultRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testTournamentResult);

        _mapperMock
            .Setup(x => x.Map<TournamentResultGetDto>(testTournamentResult))
            .Returns(testTournamentResultDto);

        // Act
        var result = await _getTournamentResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        result.Should().BeEquivalentTo(testTournamentResultDto);

        _tournamentResultRepositoryMock.Verify(
            x => x.GetByIdAsync(validId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(testTournamentResult),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidIdFormat_ThrowsInvalidIdFormatException()
    {
        // Arrange
        var invalidId = "invalid-id";
        var ct = CancellationToken.None;

        // Act
        Func<Task> act = () => _getTournamentResultByIdUseCase.ExecuteAsync(invalidId, ct);

        // Assert
        await act.Should().ThrowAsync<InvalidIdFormatException>();

        _tournamentResultRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(It.IsAny<Domain.Models.Results.TournamentResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentId_ThrowsTournamentResultNotFoundByIdException()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439012";
        var ct = CancellationToken.None;

        _tournamentResultRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentId, ct))
            .ReturnsAsync((Domain.Models.Results.TournamentResult)null);

        // Act
        Func<Task> act = () => _getTournamentResultByIdUseCase.ExecuteAsync(nonExistentId, ct);

        // Assert
        await act.Should().ThrowAsync<TournamentResultNotFoundByIdException>();

        _tournamentResultRepositoryMock.Verify(
            x => x.GetByIdAsync(nonExistentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(It.IsAny<Domain.Models.Results.TournamentResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getTournamentResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _tournamentResultRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TournamentResultGetDto>(It.IsAny<Domain.Models.Results.TournamentResult>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _tournamentResultRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTournamentResultByIdUseCase.ExecuteAsync(validId, ct);

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
        var testTournamentResult = UseCasesTestData.CreateTestTournamentResultsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _tournamentResultRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testTournamentResult);

        _mapperMock
            .Setup(x => x.Map<TournamentResultGetDto>(testTournamentResult))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTournamentResultByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}