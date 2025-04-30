using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Bets;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.Protos;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;

public sealed class PlacePayoutCommandHandler(
    IPayoutRepository payoutRepository,
    IBetRepository betRepository,
    UserGrpcService.UserGrpcServiceClient userClient)
    : IRequestHandler<RequestPayoutCommand, Unit>
{
    public async Task<Unit> Handle(RequestPayoutCommand request, CancellationToken cancellationToken)
    {
        var bet = await betRepository.GetByIdAsync(request.RequestPayoutDto.BetId, cancellationToken);
        if (bet is null)
        {
            throw new BetNotFoundByIdException(request.RequestPayoutDto.BetId);
        }

        if (!(bet.Status == BetStatus.Won))
        {
            throw new InvalidBetStatusForPayoutException(bet.Id, bet.Status);
        }

        var existingPayout = await payoutRepository.GetByBetIdAsync(request.RequestPayoutDto.BetId, cancellationToken);
        if (existingPayout is not null &&
            (existingPayout.Status == PayoutStatus.Pending
             || existingPayout.Status == PayoutStatus.Completed))
        {
            throw new PayoutAlreadyExistsException(request.RequestPayoutDto.BetId, existingPayout.Status);
        }

        var expectedAmount = Math.Round(bet.Amount * bet.Odds, BetConstants.CalculationScale);
        var roundedRequested = Math.Round(request.RequestPayoutDto.Amount, BetConstants.CalculationScale);

        if (expectedAmount != roundedRequested)
        {
            throw new InvalidAmountForPayoutException(bet.Id, request.RequestPayoutDto.Amount);
        }

        var balanceResponse = await userClient.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = request.Username },
            cancellationToken: cancellationToken);
        if (!balanceResponse.UserExists)
        {
            throw new UserNotFoundByNameException(request.Username);
        }

        var payout = new Payout
        {
            Id = Guid.NewGuid(),
            BetId = request.RequestPayoutDto.BetId,
            Username = request.Username,
            Amount = request.RequestPayoutDto.Amount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.Now.ToUniversalTime(),
        };

        payoutRepository.Create(payout);

        cancellationToken.ThrowIfCancellationRequested();

        await payoutRepository.SaveAsync(cancellationToken);

        return Unit.Value;
    }
}