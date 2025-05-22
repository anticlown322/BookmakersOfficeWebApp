using BettingService.BLL.Services.Hangfire;
using MediatR;

namespace BettingService.Tests.UnitTests.Services.Hangfire;

public class HangfireJobExecutorTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly HangfireJobExecutor _executor;

    public HangfireJobExecutorTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _executor = new HangfireJobExecutor(_mediatorMock.Object);
    }

    [Theory]
    [InlineData(HangfireJobNames.UpdatePendingBets)]
    [InlineData(HangfireJobNames.UpdateActiveBets)]
    [InlineData(HangfireJobNames.ProcessPayouts)]
    public async Task ExecuteAsync_ShouldSendCommand(string jobId)
    {
        // Act
        await _executor.ExecuteAsync(jobId);

        // Assert
        _mediatorMock.Verify(x => x.Send(
                It.IsAny<IRequest<Unit>>(), 
                It.IsAny<CancellationToken>()), 
            Times.Once);
    }
}