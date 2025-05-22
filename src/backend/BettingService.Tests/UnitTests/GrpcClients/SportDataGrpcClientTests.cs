using BettingService.Protos;
using Grpc.Core;

namespace BettingService.Tests.UnitTests.GrpcClients;

public class SportDataGrpcClientTests
{
    private readonly Mock<SportDataService.SportDataServiceClient> _clientMock;

    public SportDataGrpcClientTests()
    {
        _clientMock = new Mock<SportDataService.SportDataServiceClient>();
    }

    [Fact]
    public async Task ValidateBetAsync_ValidBet_ReturnsValidationResult()
    {
        // Arrange
        var request = new ValidateBetRequest
        {
            MatchId = "match1",
            LineType = "total",
            MarketSelection = "over",
            Odds = 2.0
        };

        var response = GrpcClientsTestData.CreateValidBetResponse();

        _clientMock.Setup(c => c.ValidateBetAsync(
                It.Is<ValidateBetRequest>(r => r.MatchId == "match1"),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<ValidateBetResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var result = await _clientMock.Object.ValidateBetAsync(
            request, 
            cancellationToken: CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        result.CurrentOdds.Should().Be(2.0);
    }

    [Fact]
    public async Task GetMatchResultAsync_MatchExists_ReturnsResult()
    {
        // Arrange
        var request = new GetMatchResultRequest { MatchId = "match1" };
        var response = GrpcClientsTestData.CreateMatchResultResponse("match1", ResultStatus.Ended);

        _clientMock.Setup(c => c.GetMatchResultAsync(
                It.Is<GetMatchResultRequest>(r => r.MatchId == "match1"),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetMatchResultResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var result = await _clientMock.Object.GetMatchResultAsync(
            request,
            cancellationToken: CancellationToken.None);

        // Assert
        result.HasResult.Should().BeTrue();
        result.Result.Status.Should().Be(ResultStatus.Ended);
    }

    [Fact]
    public async Task GetMatchResultsBatchAsync_MultipleMatches_ReturnsResults()
    {
        // Arrange
        var request = new GetMatchResultsBatchRequest 
        { 
            MatchIds = { "match1", "match2" } 
        };
        
        var response = GrpcClientsTestData.CreateBatchMatchResults("match1", "match2");

        _clientMock.Setup(c => c.GetMatchResultsBatchAsync(
                It.Is<GetMatchResultsBatchRequest>(r => 
                    r.MatchIds.Count == 2 &&
                    r.MatchIds.Contains("match1") &&
                    r.MatchIds.Contains("match2")),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetMatchResultsBatchResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var result = await _clientMock.Object.GetMatchResultsBatchAsync(
            request,
            cancellationToken: CancellationToken.None);

        // Assert
        result.Results.Should().HaveCount(2);
        result.Results[0].MatchId.Should().Be("match1");
        result.Results[1].MatchId.Should().Be("match2");
    }

    [Fact]
    public async Task ValidateBetAsync_InvalidBet_ReturnsValidationError()
    {
        // Arrange
        var request = new ValidateBetRequest
        {
            MatchId = "match1",
            LineType = "invalid",
            MarketSelection = "invalid",
            Odds = 2.0
        };

        var response = new ValidateBetResponse
        {
            IsValid = false,
            ErrorMessage = "Invalid market selection"
        };

        _clientMock.Setup(c => c.ValidateBetAsync(
                It.Is<ValidateBetRequest>(r => r.MatchId == "match1"),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<ValidateBetResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var result = await _clientMock.Object.ValidateBetAsync(
            request,
            cancellationToken: CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task GetMatchResultAsync_MatchNotFound_ReturnsNoResult()
    {
        // Arrange
        var request = new GetMatchResultRequest { MatchId = "unknown" };
        var response = new GetMatchResultResponse { HasResult = false };

        _clientMock.Setup(c => c.GetMatchResultAsync(
                It.Is<GetMatchResultRequest>(r => r.MatchId == "unknown"),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetMatchResultResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }));

        // Act
        var result = await _clientMock.Object.GetMatchResultAsync(
            request,
            cancellationToken: CancellationToken.None);

        // Assert
        result.HasResult.Should().BeFalse();
        result.Result.Should().BeNull();
    }
}