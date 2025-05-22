using BettingService.BLL.UseCases.Payouts.Commands.ProcessPayouts;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using Grpc.Core;
using MediatR;

namespace BettingService.Tests.UnitTests.UseCases.Commands.Payouts;

public class ProcessPayoutsCommandHandlerTests
{
    private readonly Mock<IPayoutRepository> _payoutRepositoryMock;
    private readonly Mock<UserGrpcService.UserGrpcServiceClient> _userGrpcClientMock;
    private readonly ProcessPayoutsCommandHandler _handler;

    public ProcessPayoutsCommandHandlerTests()
    {
        _payoutRepositoryMock = new Mock<IPayoutRepository>();
        _userGrpcClientMock = new Mock<UserGrpcService.UserGrpcServiceClient>();
        _handler = new ProcessPayoutsCommandHandler(
            _payoutRepositoryMock.Object,
            _userGrpcClientMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldDoNothing_WhenNoPendingPayouts()
    {
        // Arrange
        var command = new ProcessPayoutsCommand();
        var cancellationToken = CancellationToken.None;

        _payoutRepositoryMock
            .Setup(x => x.FindByConditionAsync(
                p => p.Status == PayoutStatus.Pending,
                true,
                cancellationToken))
            .ReturnsAsync(new List<Payout>());

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        _userGrpcClientMock.Verify(
            x => x.UpdateUserBalanceAsync(It.IsAny<UpdateUserBalanceRequest>(), It.IsAny<CallOptions>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldProcessPayouts_WhenPendingPayoutsExist()
    {
        // Arrange
        var command = new ProcessPayoutsCommand();
        var cancellationToken = CancellationToken.None;
        var username = "testUser";
        var payoutAmount = 100m;

        var pendingPayouts = new List<Payout>
        {
            new Payout
            {
                Id = Guid.NewGuid(),
                Username = username,
                Amount = payoutAmount,
                Status = PayoutStatus.Pending
            },
            new Payout
            {
                Id = Guid.NewGuid(),
                Username = username,
                Amount = payoutAmount,
                Status = PayoutStatus.Pending
            }
        };

        var response = new UpdateUserBalanceResponse { Success = true };
        
        _payoutRepositoryMock
            .Setup(x => x.FindByConditionAsync(
                p => p.Status == PayoutStatus.Pending,
                true,
                cancellationToken))
            .ReturnsAsync(pendingPayouts);

        _userGrpcClientMock
            .Setup(x => x.UpdateUserBalanceAsync(
                It.Is<UpdateUserBalanceRequest>(r => 
                    r.Username == username && 
                    r.Amount == (double)(payoutAmount * 2)),
                null,
                null,
                cancellationToken))
            .Returns(new AsyncUnaryCall<UpdateUserBalanceResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        
        pendingPayouts.ForEach(p => 
        {
            p.Status.Should().Be(PayoutStatus.Completed);
            p.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
            p.ErrorReason.Should().BeNull();
        });

        _payoutRepositoryMock.Verify(
            x => x.Update(It.IsAny<Payout>()),
            Times.Exactly(pendingPayouts.Count));

        _payoutRepositoryMock.Verify(
            x => x.SaveAsync(cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMarkAsFailed_WhenGrpcCallFails()
    {
        // Arrange
        var command = new ProcessPayoutsCommand();
        var cancellationToken = CancellationToken.None;
        var username = "testUser";

        var pendingPayouts = new List<Payout>
        {
            new Payout
            {
                Id = Guid.NewGuid(),
                Username = username,
                Amount = 100m,
                Status = PayoutStatus.Pending
            }
        };

        _payoutRepositoryMock
            .Setup(x => x.FindByConditionAsync(
                p => p.Status == PayoutStatus.Pending,
                true,
                cancellationToken))
            .ReturnsAsync(pendingPayouts);

        _userGrpcClientMock
            .Setup(x => x.UpdateUserBalanceAsync(
                It.IsAny<UpdateUserBalanceRequest>(),
                null,
                null,
                cancellationToken))
            .Returns(new AsyncUnaryCall<UpdateUserBalanceResponse>(
                Task.FromResult<UpdateUserBalanceResponse>(null!),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));
        
        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        
        pendingPayouts.ForEach(p => 
        {
            p.Status.Should().Be(PayoutStatus.Failed);
            p.ProcessedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        });

        _payoutRepositoryMock.Verify(
            x => x.SaveAsync(cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldMarkAsFailed_WhenBalanceUpdateFails()
    {
        // Arrange
        var command = new ProcessPayoutsCommand();
        var cancellationToken = CancellationToken.None;
        var username = "testUser";
        var errorMessage = "Balance update failed";

        var pendingPayouts = new List<Payout>
        {
            new Payout
            {
                Id = Guid.NewGuid(),
                Username = username,
                Amount = 100m,
                Status = PayoutStatus.Pending
            }
        };
        
        var response = new UpdateUserBalanceResponse { Success = false };

        _payoutRepositoryMock
            .Setup(x => x.FindByConditionAsync(
                p => p.Status == PayoutStatus.Pending,
                true,
                cancellationToken))
            .ReturnsAsync(pendingPayouts);

        _userGrpcClientMock
            .Setup(x => x.UpdateUserBalanceAsync(
                It.IsAny<UpdateUserBalanceRequest>(),
                null,
                null,
                cancellationToken))
            .Returns(new AsyncUnaryCall<UpdateUserBalanceResponse>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => [],
                () => { }
            ));

        // Act
        var result = await _handler.Handle(command, cancellationToken);

        // Assert
        result.Should().Be(Unit.Value);
        
        pendingPayouts.ForEach(p => 
        {
            p.Status.Should().Be(PayoutStatus.Failed);
            p.ErrorReason.Should().Be(errorMessage);
        });
    }
}