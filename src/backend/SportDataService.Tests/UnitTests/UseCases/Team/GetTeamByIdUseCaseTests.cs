using AutoMapper;
using Moq;
using SportDataService.Application.DTO.Common;
using SportDataService.Application.UseCases.Team;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.Team;

public class GetTeamByIdUseCaseTests
{
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly GetTeamByIdUseCase _getTeamByIdUseCase;

    public GetTeamByIdUseCaseTests()
    {
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _mapperMock = new Mock<IMapper>();
        _getTeamByIdUseCase = new GetTeamByIdUseCase(_teamRepositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ValidId_ReturnsMappedTeam()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var testTeam = TeamUseCasesTestData.CreateTestTeamsWithMetadata(1).First();
        var testTeamDto = TeamUseCasesTestData.CreateTestTeamDtos(1).First();

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testTeam);

        _mapperMock
            .Setup(x => x.Map<TeamGetDto>(testTeam))
            .Returns(testTeamDto);

        // Act
        var result = await _getTeamByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        result.Should().BeEquivalentTo(testTeamDto);

        _teamRepositoryMock.Verify(
            x => x.GetByIdAsync(validId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(testTeam),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_InvalidIdFormat_ThrowsInvalidIdFormatException()
    {
        // Arrange
        var invalidId = "invalid-id";
        var ct = CancellationToken.None;

        // Act
        Func<Task> act = () => _getTeamByIdUseCase.ExecuteAsync(invalidId, ct);

        // Assert
        await act.Should().ThrowAsync<InvalidIdFormatException>();

        _teamRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(It.IsAny<Domain.Models.Common.Team>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_NonExistentId_ThrowsTeamNotFoundByIdException()
    {
        // Arrange
        var nonExistentId = "507f1f77bcf86cd799439012";
        var ct = CancellationToken.None;

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(nonExistentId, ct))
            .ReturnsAsync((Domain.Models.Common.Team)null);

        // Act
        Func<Task> act = () => _getTeamByIdUseCase.ExecuteAsync(nonExistentId, ct);

        // Assert
        await act.Should().ThrowAsync<TeamNotFoundByIdException>();

        _teamRepositoryMock.Verify(
            x => x.GetByIdAsync(nonExistentId, ct),
            Times.Once);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(It.IsAny<Domain.Models.Common.Team>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _getTeamByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();

        _teamRepositoryMock.Verify(
            x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _mapperMock.Verify(
            x => x.Map<TeamGetDto>(It.IsAny<Domain.Models.Common.Team>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_RepositoryThrowsException_PropagatesException()
    {
        // Arrange
        var validId = "507f1f77bcf86cd799439011";
        var ct = CancellationToken.None;
        var expectedException = new Exception("Database error");

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _getTeamByIdUseCase.ExecuteAsync(validId, ct);

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
        var testTeam = TeamUseCasesTestData.CreateTestTeamsWithMetadata(1).First();
        var expectedException = new AutoMapperMappingException("Mapping failed");

        _teamRepositoryMock
            .Setup(x => x.GetByIdAsync(validId, ct))
            .ReturnsAsync(testTeam);

        _mapperMock
            .Setup(x => x.Map<TeamGetDto>(testTeam))
            .Throws(expectedException);

        // Act
        Func<Task> act = () => _getTeamByIdUseCase.ExecuteAsync(validId, ct);

        // Assert
        var exception = await act.Should().ThrowAsync<AutoMapperMappingException>();

        exception.Which.Message.Should().Be(expectedException.Message);
    }
}