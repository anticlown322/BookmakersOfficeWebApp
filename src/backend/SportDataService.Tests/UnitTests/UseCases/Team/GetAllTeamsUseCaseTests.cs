using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Common;
using SportDataService.Application.UseCases.Team;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Tests.UnitTests.UseCases.Team;

public class GetAllTeamsUseCaseTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetAllTeamsUseCase _getAllTeamsUseCase;

    public GetAllTeamsUseCaseTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _mapperMock = new Mock<IMapper>();
        _getAllTeamsUseCase = new GetAllTeamsUseCase(_teamRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ReturnsMappedTeamsAndMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TeamParameters();
        var testTeams = TeamUseCasesTestData.CreateTestTeamsWithMetadata(3);
        var testTeamDtos = TeamUseCasesTestData.CreateTestTeamDtos(3);

        _teamRepositoryMock
            .Setup(x => x.FindAllTeamsAsync(parameters, ct))
            .ReturnsAsync(testTeams);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TeamGetDto>>(testTeams))
            .Returns(testTeamDtos);

        // Act
        var result = await _getAllTeamsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.teams.Should().BeEquivalentTo(testTeamDtos);
        result.metaData.Should().Be(testTeams.MetaData);

        _teamRepositoryMock.Verify(
            x => x.FindAllTeamsAsync(parameters, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<IEnumerable<TeamGetDto>>(testTeams),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_EmptyResult_ReturnsEmptyCollectionWithMetadata()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TeamParameters();
        var emptyTeams = TeamUseCasesTestData.CreateTestTeamsWithMetadata(0);
        var emptyDtos = TeamUseCasesTestData.CreateTestTeamDtos(0);

        _teamRepositoryMock
            .Setup(x => x.FindAllTeamsAsync(parameters, ct))
            .ReturnsAsync(emptyTeams);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TeamGetDto>>(emptyTeams))
            .Returns(emptyDtos);

        // Act
        var result = await _getAllTeamsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        result.teams.Should().BeEmpty();
        result.metaData.Should().Be(emptyTeams.MetaData);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);
        var parameters = new TeamParameters();

        // Act
        Func<Task> act = () => _getAllTeamsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _teamRepositoryMock.Verify(
            x => x.FindAllTeamsAsync(It.IsAny<TeamParameters>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TeamParameters();
        var expectedException = new Exception("Database error");

        _teamRepositoryMock
            .Setup(x => x.FindAllTeamsAsync(parameters, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getAllTeamsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var ct = CancellationToken.None;
        var parameters = new TeamParameters();
        var testTeams = TeamUseCasesTestData.CreateTestTeamsWithMetadata(2);
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _teamRepositoryMock
            .Setup(x => x.FindAllTeamsAsync(parameters, ct))
            .ReturnsAsync(testTeams);

        _mapperMock
            .Setup(x => x.Map<IEnumerable<TeamGetDto>>(testTeams))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getAllTeamsUseCase.ExecuteAsync(parameters, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}