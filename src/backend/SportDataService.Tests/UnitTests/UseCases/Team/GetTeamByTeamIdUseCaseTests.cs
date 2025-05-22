using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Common;
using SportDataService.Application.UseCases.Team;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.Team;

public class GetTeamByTeamIdUseCaseTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTeamByTeamIdUseCase _getTeamByTeamIdUseCase;

    public GetTeamByTeamIdUseCaseTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTeamByTeamIdUseCase = new GetTeamByTeamIdUseCase(_teamRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidTeamId_ReturnsMappedTeam()
    {
        // Arrange
        var validTeamId = "50012345";
        var ct = CancellationToken.None;
        var testTeam = TeamUseCasesTestData.CreateTestTeamsWithMetadata(1).First();
        var testTeamDto = TeamUseCasesTestData.CreateTestTeamDtos(1).First();

        _teamRepositoryMock
            .Setup(x => x.GetTeamByTeamIdAsync(validTeamId, ct))
            .ReturnsAsync(testTeam);

        _mapperMock
            .Setup(x => x.Map<TeamGetDto>(testTeam))
            .Returns(testTeamDto);

        // Act
        var result = await _getTeamByTeamIdUseCase.ExecuteAsync(validTeamId, ct);

        // Assert
        result.Should().BeEquivalentTo(testTeamDto);

        _teamRepositoryMock.Verify(
            x => x.GetTeamByTeamIdAsync(validTeamId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(testTeam),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentTeamId_ThrowsTeamNotFoundByTeamIdException()
    {
        // Arrange
        var nonExistentTeamId = "50099999";
        var ct = CancellationToken.None;

        _teamRepositoryMock
            .Setup(x => x.GetTeamByTeamIdAsync(nonExistentTeamId, ct))
            .ReturnsAsync((Domain.Models.Common.Team)null);

        // Act
        Func<Task> act = () => _getTeamByTeamIdUseCase.ExecuteAsync(nonExistentTeamId, ct);

        // Assert
        await act.Should().ThrowAsync<TeamNotFoundByTeamIdException>();

        _teamRepositoryMock.Verify(
            x => x.GetTeamByTeamIdAsync(nonExistentTeamId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(It.IsAny<Domain.Models.Common.Team>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validTeamId = "50012345";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getTeamByTeamIdUseCase.ExecuteAsync(validTeamId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _teamRepositoryMock.Verify(
            x => x.GetTeamByTeamIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(It.IsAny<Domain.Models.Common.Team>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validTeamId = "50012345";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _teamRepositoryMock
            .Setup(x => x.GetTeamByTeamIdAsync(validTeamId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTeamByTeamIdUseCase.ExecuteAsync(validTeamId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<Exception>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }

    [Fact]
    public async Task ExecuteAsync_MapperThrowsException_PropagatesException()
    {
        // Arrange
        var validTeamId = "50012345";
        var ct = CancellationToken.None;
        var testTeam = TeamUseCasesTestData.CreateTestTeamsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _teamRepositoryMock
            .Setup(x => x.GetTeamByTeamIdAsync(validTeamId, ct))
            .ReturnsAsync(testTeam);

        _mapperMock
            .Setup(x => x.Map<TeamGetDto>(testTeam))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTeamByTeamIdUseCase.ExecuteAsync(validTeamId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}