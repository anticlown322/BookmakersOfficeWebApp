using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.UseCases.Tournament;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.Tournament;

public class GetTournamentByIdUseCaseTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTournamentByIdUseCase _getTournamentByIdUseCase;

    public GetTournamentByIdUseCaseTests()
    {
        _tournamentRepositoryMock = new Mock<ITournamentRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTournamentByIdUseCase = new GetTournamentByIdUseCase(_tournamentRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidId_ReturnsMappedTournament()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var testTournament = UseCasesTestData.CreateTestTournamentsWithMetadata(1).First();
        var testTournamentDto = UseCasesTestData.CreateTestTournamentDtos(1).First();

        _tournamentRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testTournament);

        _mapperMock
            .Setup(x => x.Map<TournamentGetDto>(testTournament))
            .Returns(testTournamentDto);

        // Act
        var result = await _getTournamentByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        result.Should().BeEquivalentTo(testTournamentDto);

        _tournamentRepositoryMock.Verify(
            x => x.GetByIdAsync(validId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(testTournament),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidIdFormat_ThrowsInvalidIdFormatException()
    {
        // Arrange
        var invalidId = "invalid-id";
        var ct = CancellationToken.None;

        // Act
        Func<Task> act = () => _getTournamentByIdUseCase.ExecuteAsync(invalidId, ct);

        // Assert
        await act.Should().ThrowAsync<InvalidIdFormatException>();

        _tournamentRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(It.IsAny<Domain.Models.Prematch.Tournament>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentId_ThrowsTournamentNotFoundByIdException()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439012";
        var ct = CancellationToken.None;

        _tournamentRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentId, ct))
            .ReturnsAsync((Domain.Models.Prematch.Tournament)null);

        // Act
        Func<Task> act = () => _getTournamentByIdUseCase.ExecuteAsync(nonExistentId, ct);

        // Assert
        await act.Should().ThrowAsync<TournamentNotFoundByIdException>();

        _tournamentRepositoryMock.Verify(
            x => x.GetByIdAsync(nonExistentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(It.IsAny<Domain.Models.Prematch.Tournament>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getTournamentByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _tournamentRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(It.IsAny<Domain.Models.Prematch.Tournament>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _tournamentRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTournamentByIdUseCase.ExecuteAsync(validId, ct);

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
        var testTournament = UseCasesTestData.CreateTestTournamentsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _tournamentRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testTournament);

        _mapperMock
            .Setup(x => x.Map<TournamentGetDto>(testTournament))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTournamentByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}