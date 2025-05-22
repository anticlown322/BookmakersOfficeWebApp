using Moq;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Infrastructure.Services.Hangfire;

namespace SportDataService.Tests.UnitTests.Services.Hangfire;

public class HangfireJobExecutorTests
{
    private readonly Mock<IRefreshTournamentsUseCase> _refreshTournamentsMock;
    private readonly Mock<IRefreshTournamentResultsUseCase> _refreshResultsMock;
    private readonly HangfireJobExecutor _executor;

    public HangfireJobExecutorTests()
    {
        _refreshTournamentsMock = new Mock<IRefreshTournamentsUseCase>();
        _refreshResultsMock = new Mock<IRefreshTournamentResultsUseCase>();
        _executor = new HangfireJobExecutor(
            _refreshTournamentsMock.Object,
            _refreshResultsMock.Object);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRefreshTournaments_ForUpdatePrematchJob()
    {
        // Arrange
        var jobId = HangfireJobNames.UpdatePrematch;
        _refreshTournamentsMock
            .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _executor.ExecuteAsync(jobId);

        // Assert
        _refreshTournamentsMock.Verify(
            x => x.ExecuteAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _refreshResultsMock.Verify(
            x => x.ExecuteAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCallRefreshTournamentResults_ForUpdateResultsJob()
    {
        // Arrange
        var jobId = HangfireJobNames.UpdateResults;
        _refreshResultsMock
            .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        await _executor.ExecuteAsync(jobId);

        // Assert
        _refreshResultsMock.Verify(
            x => x.ExecuteAsync(It.IsAny<CancellationToken>()), 
            Times.Once);
        
        _refreshTournamentsMock.Verify(
            x => x.ExecuteAsync(It.IsAny<CancellationToken>()), 
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldHandleUseCaseExceptions()
    {
        // Arrange
        var jobId = HangfireJobNames.UpdateResults;
        var expectedException = new InvalidOperationException("Test error");
        _refreshResultsMock
            .Setup(x => x.ExecuteAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(expectedException);

        // Act
        Func<Task> act = () => _executor.ExecuteAsync(jobId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage(expectedException.Message);
    }
}