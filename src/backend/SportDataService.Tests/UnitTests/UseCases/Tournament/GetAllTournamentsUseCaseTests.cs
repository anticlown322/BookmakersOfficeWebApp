using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.UseCases.Tournament;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Tests.UnitTests.UseCases.Tournament;

public class GetAllTournamentsUseCaseTests
{
    private readonly Mock<ITournamentRepository> _tournamentRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTournamentsUseCase _getAllTournamentsUseCase;

    public GetAllTournamentsUseCaseTests()
    {
        _tournamentRepositoryMock = new Mock<ITournamentRepository>();
        _mapperMock = new Mock<IMapper>();
        _getAllTournamentsUseCase = new GetAllTournamentsUseCase(_tournamentRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedTournamentsAndMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentParameters();
        var testTournaments = TournamentUseCasesTestData.CreateTestTournamentsWithMetadata(3);
        var testTournamentDtos = TournamentUseCasesTestData.CreateTestTournamentDtos(3);

        _tournamentRepositoryMock
            .Setup(x => x.FindAllTournamentsAsync(parameters, ct))
            .ReturnsAsync(testTournaments);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TournamentGetDto>>(testTournaments))
            .Returns(testTournamentDtos);

        // Act
        var result = await _getAllTournamentsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.tournaments.Should().BeEquivalentTo(testTournamentDtos);
        result.metaData.Should().Be(testTournaments.MetaData);

        _tournamentRepositoryMock.Verify(
            x => x.FindAllTournamentsAsync(parameters, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<TournamentGetDto>>(testTournaments),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyResult_ReturnsEmptyCollectionWithMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentParameters();
        var emptyTournaments = TournamentUseCasesTestData.CreateTestTournamentsWithMetadata(0);
        var emptyDtos = TournamentUseCasesTestData.CreateTestTournamentDtos(0);

        _tournamentRepositoryMock
            .Setup(x => x.FindAllTournamentsAsync(parameters, ct))
            .ReturnsAsync(emptyTournaments);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TournamentGetDto>>(emptyTournaments))
            .Returns(emptyDtos);

        // Act
        var result = await _getAllTournamentsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.tournaments.Should().BeEmpty();
        result.metaData.Should().Be(emptyTournaments.MetaData);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);
        var parameters = new TournamentParameters();

        // Act
        Func<Task> act = () => _getAllTournamentsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _tournamentRepositoryMock.Verify(
            x => x.FindAllTournamentsAsync(It.IsAny<TournamentParameters>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentParameters();
        var expectedException = new Exception("Database error");

        _tournamentRepositoryMock
            .Setup(x => x.FindAllTournamentsAsync(parameters, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getAllTournamentsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentParameters();
        var testTournaments = TournamentUseCasesTestData.CreateTestTournamentsWithMetadata(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _tournamentRepositoryMock
            .Setup(x => x.FindAllTournamentsAsync(parameters, ct))
            .ReturnsAsync(testTournaments);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TournamentGetDto>>(testTournaments))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getAllTournamentsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}