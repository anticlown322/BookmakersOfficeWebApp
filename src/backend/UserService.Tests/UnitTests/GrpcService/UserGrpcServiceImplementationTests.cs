using Grpc.Core;
using UserService.Domain.Models;
using UserService.Domain.RepositoryContracts;
using UserService.GrpcService.Services;

namespace UserService.Tests.UnitTests.GrpcService;

public class UserGrpcServiceImplementationTests
{
    private readonly Mock<ServerCallContext> _contextMock;
    private readonly Mock<IUsersRepository> _mockRepository;
    private readonly UserGrpcServiceImplementation _service;

    public UserGrpcServiceImplementationTests()
    {
        _contextMock = new Mock<ServerCallContext>();
        _mockRepository = new Mock<IUsersRepository>();
        _service = new UserGrpcServiceImplementation(_mockRepository.Object);
    }
    
    [Fact]
    public async Task GetUserBalance_UserExists_ReturnsBalance()
    {
        // Arrange
        var request = new GetUserBalanceRequest { Username = "testUser" };
        _mockRepository
            .Setup(x => x.GetUserByNameAsync(request.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserGrpcTestData.ValidUser);

        // Act
        var response = await _service.GetUserBalance(request, _contextMock.Object);

        // Assert
        response.UserExists.Should().BeTrue();
        response.Balance.Should().Be(100.50);
    }
    
    [Fact]
    public async Task GetUserBalance_UserNotExists_ReturnsFalse()
    {
        // Arrange
        var request = new GetUserBalanceRequest { Username = "unknownUser" };
        _mockRepository
            .Setup(x => x.GetUserByNameAsync(request.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var response = await _service.GetUserBalance(request, _contextMock.Object);

        // Assert
        response.UserExists.Should().BeFalse();
        response.Balance.Should().Be(0);
    }
    
    [Fact]
    public async Task GetUserBalance_RepositoryThrows_ThrowsRpcException()
    {
        // Arrange
        var request = new GetUserBalanceRequest { Username = "testUser" };
        _mockRepository
            .Setup(x => x.GetUserByNameAsync(request.Username, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act and Assert
        var ex = await FluentActions.Invoking(() => 
            _service.GetUserBalance(request, _contextMock.Object))
            .Should().ThrowAsync<RpcException>();

        ex.Which.StatusCode.Should().Be(StatusCode.Internal);
    }
    
    [Fact]
    public async Task UpdateUserBalance_ValidAmount_UpdatesBalance()
    {
        // Arrange
        var request = new UpdateUserBalanceRequest
        {
            Username = "testUser",
            Amount = 50.25
        };
        var ct = CancellationToken.None;

        _mockRepository
            .Setup(x => x.GetUserByNameAsync(request.Username, ct))
            .ReturnsAsync(UserGrpcTestData.ValidUser);

        // Act
        var response = await _service.UpdateUserBalance(request, _contextMock.Object);

        // Assert
        response.Success.Should().BeTrue();
        response.NewBalance.Should().Be(150.75); // 100.50 + 50.25
        _mockRepository.Verify(x => x.UpdateUserAsync(It.IsAny<User>(), ct), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUserBalance_InsufficientFunds_ThrowsRpcException()
    {
        // Arrange
        var request = new UpdateUserBalanceRequest
        {
            Username = "testUser",
            Amount = -200.00
        };

        _mockRepository
            .Setup(x => x.GetUserByNameAsync(request.Username, It.IsAny<CancellationToken>()))
            .ReturnsAsync(UserGrpcTestData.ValidUser);

        // Act and Assert
        var ex = await FluentActions.Invoking(() => 
            _service.UpdateUserBalance(request, _contextMock.Object))
            .Should().ThrowAsync<RpcException>();

        ex.Which.StatusCode.Should().Be(StatusCode.FailedPrecondition);
    }
}