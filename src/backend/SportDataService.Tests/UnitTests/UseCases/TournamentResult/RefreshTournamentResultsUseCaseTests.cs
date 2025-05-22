using Moq;
using SportDataService.Application.Contracts.Services;
using SportDataService.Application.UseCases.TournamentResult;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Tests.UnitTests.UseCases.TournamentResult;

public class RefreshTournamentResultsUseCaseTests
{
    private readonly Mock<IDataCollectionService> _dataCollectionServiceMock;
    private readonly Mock<ITournamentResultRepository> _tournamentResultRepositoryMock;
    private readonly Mock<IMatchResultRepository> _matchResultRepositoryMock;
    private readonly Mock<ITeamRepository> _teamRepositoryMock;
    private readonly RefreshTournamentResultsUseCase _refreshUseCase;

    public RefreshTournamentResultsUseCaseTests()
    {
        _dataCollectionServiceMock = new Mock<IDataCollectionService>();
        _tournamentResultRepositoryMock = new Mock<ITournamentResultRepository>();
        _matchResultRepositoryMock = new Mock<IMatchResultRepository>();
        _teamRepositoryMock = new Mock<ITeamRepository>();
        _refreshUseCase = new RefreshTournamentResultsUseCase(
            _dataCollectionServiceMock.Object,
            _tournamentResultRepositoryMock.Object,
            _matchResultRepositoryMock.Object,
            _teamRepositoryMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_WithNewTournament_CreatesAllEntities()
    {
        // Arrange
        var ct = CancellationToken.None;
        var testTournament = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(1).First();
        
        _dataCollectionServiceMock
            .Setup(x => x.GetTournamentsResultsInfoAsync(ct))
            .ReturnsAsync(new List<Domain.Models.Results.TournamentResult> { testTournament });

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(testTournament.TournamentId, ct))
            .ReturnsAsync((Domain.Models.Results.TournamentResult)null);

        // Act
        await _refreshUseCase.ExecuteAsync(ct);

        // Assert
        _teamRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Domain.Models.Common.Team>(), ct), Times.Exactly(2));
        _matchResultRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Domain.Models.Results.MatchResult>(), ct), Times.Once);
        _tournamentResultRepositoryMock.Verify(x => x.CreateAsync(It.IsAny<Domain.Models.Results.TournamentResult>(), ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithExistingTournament_UpdatesEntities()
    {
        // Arrange
        var ct = CancellationToken.None;
        var tournaments = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(2);
        var existingTournament = tournaments[0];
        var newTournament = tournaments[1];
        newTournament.TournamentName = "Updated Name";
        
        _dataCollectionServiceMock
            .Setup(x => x.GetTournamentsResultsInfoAsync(ct))
            .ReturnsAsync(new List<Domain.Models.Results.TournamentResult> { newTournament });

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(newTournament.TournamentId, ct))
            .ReturnsAsync(existingTournament);

        // Act
        await _refreshUseCase.ExecuteAsync(ct);

        // Assert
        _tournamentResultRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<Domain.Models.Results.TournamentResult>(t => t.TournamentName == "Updated Name"), 
            ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        // Arrange
        var ct = new CancellationToken(canceled: true);

        // Act
        Func<Task> act = () => _refreshUseCase.ExecuteAsync(ct);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task ExecuteAsync_WithRemovedMatches_DeletesOldMatches()
    {
        // Arrange
        var ct = CancellationToken.None;
        var tournaments = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(2);
        var existingTournament = tournaments[0];
        var newTournament = tournaments[1];
        newTournament.Matches.Clear();
        
        _dataCollectionServiceMock
            .Setup(x => x.GetTournamentsResultsInfoAsync(ct))
            .ReturnsAsync(new List<Domain.Models.Results.TournamentResult> { newTournament });

        _tournamentResultRepositoryMock
            .Setup(x => x.GetTournamentResultByTournamentResultIdAsync(newTournament.TournamentId, ct))
            .ReturnsAsync(existingTournament);

        // Act
        await _refreshUseCase.ExecuteAsync(ct);

        // Assert
        _matchResultRepositoryMock.Verify(x => 
            x.DeleteAsync(It.IsAny<string>(), ct), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithTeamNameChange_UpdatesTeam()
    {
        // Arrange
        var ct = CancellationToken.None;
        var testTournament = TournamentResultUseCasesTestData.CreateTestTournamentResultsWithMetadata(1).First();
        var existingTeam = TournamentResultUseCasesTestData.CreateTestTeamsWithMetadata(1).First();
        existingTeam.Name = "Old Name";
        testTournament.Matches[0].Team1.Name = "New Name";
        
        _dataCollectionServiceMock
            .Setup(x => x.GetTournamentsResultsInfoAsync(ct))
            .ReturnsAsync(new List<Domain.Models.Results.TournamentResult> { testTournament });

        _teamRepositoryMock
            .Setup(x => x.GetTeamByTeamIdAsync(testTournament.Matches[0].Team1.TeamId, ct))
            .ReturnsAsync(existingTeam);

        // Act
        await _refreshUseCase.ExecuteAsync(ct);

        // Assert
        _teamRepositoryMock.Verify(x => x.UpdateAsync(
            It.Is<Domain.Models.Common.Team>(t => t.Name == "New Name"), 
            ct), Times.Once);
    }
}