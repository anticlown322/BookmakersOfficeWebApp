using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.UseCases.Tournament;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.Tournament;

public class GetTournamentByTournamentIdUseCaseTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTournamentByTournamentIdUseCase _getTournamentByTournamentIdUseCase;

    public GetTournamentByTournamentIdUseCaseTests()
    {
        _tournamentRepositoryMock = new Mock<ITournamentRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTournamentByTournamentIdUseCase = new GetTournamentByTournamentIdUseCase(_tournamentRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidTournamentId_ReturnsMappedTournament()
    {
        // Arrange
        var validTournamentId = "10012345";
        var ct = CancellationToken.None;
        var testTournament = UseCasesTestData.CreateTestTournamentsWithMetadata(1).First();
        var testTournamentDto = UseCasesTestData.CreateTestTournamentDtos(1).First();

        _tournamentRepositoryMock
            .Setup(x => x.GetTournamentByTournamentIdAsync(validTournamentId, ct))
            .ReturnsAsync(testTournament);

        _mapperMock
            .Setup(x => x.Map<TournamentGetDto>(testTournament))
            .Returns(testTournamentDto);

        // Act
        var result = await _getTournamentByTournamentIdUseCase.ExecuteAsync(validTournamentId, ct);

        // Assert
        result.Should().BeEquivalentTo(testTournamentDto);

        _tournamentRepositoryMock.Verify(
            x => x.GetTournamentByTournamentIdAsync(validTournamentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(testTournament),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentTournamentId_ThrowsTournamentNotFoundByTournamentIdException()
    {
        // Arrange
        var nonExistentTournamentId = "10099999";
        var ct = CancellationToken.None;

        _tournamentRepositoryMock
            .Setup(x => x.GetTournamentByTournamentIdAsync(nonExistentTournamentId, ct))
            .ReturnsAsync((Domain.Models.Prematch.Tournament)null);

        // Act
        Func<Task> act = () => _getTournamentByTournamentIdUseCase.ExecuteAsync(nonExistentTournamentId, ct);

        // Assert
        await act.Should().ThrowAsync<TournamentNotFoundByTournamentIdException>();

        _tournamentRepositoryMock.Verify(
            x => x.GetTournamentByTournamentIdAsync(nonExistentTournamentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(It.IsAny<Domain.Models.Prematch.Tournament>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validTournamentId = "10012345";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getTournamentByTournamentIdUseCase.ExecuteAsync(validTournamentId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _tournamentRepositoryMock.Verify(
            x => x.GetTournamentByTournamentIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TournamentGetDto>(It.IsAny<Domain.Models.Prematch.Tournament>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validTournamentId = "10012345";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _tournamentRepositoryMock
            .Setup(x => x.GetTournamentByTournamentIdAsync(validTournamentId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTournamentByTournamentIdUseCase.ExecuteAsync(validTournamentId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var validTournamentId = "10012345";
        var ct = CancellationToken.None;
        var testTournament = UseCasesTestData.CreateTestTournamentsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _tournamentRepositoryMock
            .Setup(x => x.GetTournamentByTournamentIdAsync(validTournamentId, ct))
            .ReturnsAsync(testTournament);

        _mapperMock
            .Setup(x => x.Map<TournamentGetDto>(testTournament))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTournamentByTournamentIdUseCase.ExecuteAsync(validTournamentId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}