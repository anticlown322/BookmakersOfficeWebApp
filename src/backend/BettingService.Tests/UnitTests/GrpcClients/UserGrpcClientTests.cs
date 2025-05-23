using BettingService.Protos;
using Grpc.Core;

namespace BettingService.Tests.UnitTests.GrpcClients;

public class UserGrpcClientTests
{
    private readonly Mock<UserGrpcService.UserGrpcServiceClient> _clientMock;

    public UserGrpcClientTests()
    {
        _clientMock = new Mock<UserGrpcService.UserGrpcServiceClient>();
    }

    [Fact]
    public async Task GetUserBalanceAsync_ValidUser_ReturnsBalance()
    {
        // Arrange
        const string username = "testUser";
        const double expectedBalance = 100.0;
        var response = GrpcClientsTestData.CreateValidBalanceResponse(expectedBalance);

        var call = new AsyncUnaryCall<GetUserBalanceResponse>(
            Task.FromResult(response),
            Task.FromResult(new Metadata()),
            () => Status.DefaultSuccess,
            () => new Metadata(),
            () => { });

        _clientMock
            .Setup(x => x.GetUserBalanceAsync(
                It.Is<GetUserBalanceRequest>(r => r.Username == username),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(call);

        // Act
        var result = await _clientMock.Object.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = username },
            cancellationToken: CancellationToken.None);

        // Assert
        result.Balance.Should().Be(expectedBalance);
        result.UserExists.Should().BeTrue();
    }

    [Fact]
    public async Task GetUserBalanceAsync_InvalidUser_ReturnsUserExistsFalse()
    {
        // Arrange
        const string username = "nonExistingUser";
        var response = new GetUserBalanceResponse { UserExists = false };

        _clientMock.Setup(c => c.GetUserBalanceAsync(
                It.Is<GetUserBalanceRequest>(r => r.Username == username),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetUserBalanceResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        var result = await _clientMock.Object.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = username },
            cancellationToken: CancellationToken.None);

        // Assert
        result.UserExists.Should().BeFalse();
    }

    [Fact]
    public async Task GetUserBalanceAsync_InvalidUser_ThrowsException_WhenChecked()
    {
        // Arrange
        const string username = "nonExistingUser";
        var response = new GetUserBalanceResponse { UserExists = false };

        _clientMock.Setup(c => c.GetUserBalanceAsync(
                It.Is<GetUserBalanceRequest>(r => r.Username == username),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<GetUserBalanceResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        Func<Task> act = async () =>
        {
            var result = await _clientMock.Object.GetUserBalanceAsync(
                new GetUserBalanceRequest { Username = username },
                cancellationToken: CancellationToken.None);

            if (!result.UserExists)
            {
                throw new InvalidOperationException($"User {username} not found");
            }
        };

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"User {username} not found");
    }

    [Fact]
    public async Task UpdateUserBalanceAsync_SuccessfulUpdate_ReturnsResponseWithNewBalance()
    {
        // Arrange
        const string username = "testUser";
        const double amount = 50.0;
        const double expectedNewBalance = 150.0;
        var response = GrpcClientsTestData.CreateUpdateBalanceResponse(true, expectedNewBalance);

        _clientMock.Setup(c => c.UpdateUserBalanceAsync(
                It.Is<UpdateUserBalanceRequest>(r =>
                    r.Username == username &&
                    r.Amount == amount),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<UpdateUserBalanceResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        var result = await _clientMock.Object.UpdateUserBalanceAsync(
            new UpdateUserBalanceRequest
            {
                Username = username,
                Amount = amount
            },
            cancellationToken: CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.NewBalance.Should().Be(expectedNewBalance);
    }

    [Fact]
    public async Task UpdateUserBalanceAsync_FailedUpdate_ReturnsUnsuccessfulResponse()
    {
        // Arrange
        const string username = "testUser";
        const double amount = -1000.0;
        var response = GrpcClientsTestData.CreateUpdateBalanceResponse(false);

        _clientMock.Setup(c => c.UpdateUserBalanceAsync(
                It.Is<UpdateUserBalanceRequest>(r =>
                    r.Username == username &&
                    r.Amount == amount),
                null,
                null,
                It.IsAny<CancellationToken>()))
            .Returns(new AsyncUnaryCall<UpdateUserBalanceResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        var result = await _clientMock.Object.UpdateUserBalanceAsync(
            new UpdateUserBalanceRequest
            {
                Username = username,
                Amount = amount
            },
            cancellationToken: CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
    }
}