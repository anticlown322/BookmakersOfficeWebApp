using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.UseCases.TournamentResult;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Tests.UnitTests.UseCases.TournamentResult;

public class GetAllTournamentResultsUseCaseTests
{
    private readonly Mock<ITournamentResultRepository> _tournamentResultRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTournamentResultsUseCase _getAllTournamentResultsUseCase;

    public GetAllTournamentResultsUseCaseTests()
    {
        _tournamentResultRepositoryMock = new Mock<ITournamentResultRepository>();
        _mapperMock = new Mock<IMapper>();
        _getAllTournamentResultsUseCase = new GetAllTournamentResultsUseCase(
            _tournamentResultRepositoryMock.Object, 
            _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedTournamentResultsAndMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentResultParameters();
        var testResults = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(3);
        var testResultDtos = TournamentResultUseCasesTestData.CreateTestTournamentResultDtos(3);

        _tournamentResultRepositoryMock
            .Setup(x => x.FindAllTournamentResultsAsync(parameters, ct))
            .ReturnsAsync(testResults);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TournamentResultGetDto>>(testResults))
            .Returns(testResultDtos);

        // Act
        var result = await _getAllTournamentResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.tournamentResults.Should().BeEquivalentTo(testResultDtos);
        result.metaData.Should().Be(testResults.MetaData);

        _tournamentResultRepositoryMock.Verify(
            x => x.FindAllTournamentResultsAsync(parameters, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<TournamentResultGetDto>>(testResults),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyResult_ReturnsEmptyCollectionWithMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentResultParameters();
        var emptyResults = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(0);
        var emptyDtos = TournamentResultUseCasesTestData.CreateTestTournamentResultDtos(0);

        _tournamentResultRepositoryMock
            .Setup(x => x.FindAllTournamentResultsAsync(parameters, ct))
            .ReturnsAsync(emptyResults);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TournamentResultGetDto>>(emptyResults))
            .Returns(emptyDtos);

        // Act
        var result = await _getAllTournamentResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.tournamentResults.Should().BeEmpty();
        result.metaData.Should().Be(emptyResults.MetaData);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);
        var parameters = new TournamentResultParameters();

        // Act
        Func<Task> act = () => _getAllTournamentResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _tournamentResultRepositoryMock.Verify(
            x => x.FindAllTournamentResultsAsync(It.IsAny<TournamentResultParameters>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentResultParameters();
        var expectedException = new Exception("Database error");

        _tournamentResultRepositoryMock
            .Setup(x => x.FindAllTournamentResultsAsync(parameters, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getAllTournamentResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TournamentResultParameters();
        var testResults = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _tournamentResultRepositoryMock
            .Setup(x => x.FindAllTournamentResultsAsync(parameters, ct))
            .ReturnsAsync(testResults);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TournamentResultGetDto>>(testResults))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getAllTournamentResultsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}