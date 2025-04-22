using BettingService.BLL.Exceptions.Specific;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using MediatR;

namespace BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;

public sealed class PlacePayoutCommandHandler(
    IPayoutRepository payoutRepository,
    IBetRepository betRepository)
    : IRequestHandler<RequestPayoutCommand>
{
    public async Task Handle(RequestPayoutCommand request, CancellationToken cancellationToken)
    {
        var bet = await betRepository.GetByIdAsync(request.CreatePayoutDto.BetId, cancellationToken);
        if (bet is null)
        {
            throw new BetNotFoundByIdException(request.CreatePayoutDto.BetId);
        }

        var payout = new Payout
        {
            Id = Guid.NewGuid(),
            BetId = request.CreatePayoutDto.BetId,
            Username = request.Username,
            Amount = request.CreatePayoutDto.Amount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.Now.ToUniversalTime(),
        };

        payoutRepository.Create(payout);

        cancellationToken.ThrowIfCancellationRequested();

        await payoutRepository.SaveAsync(cancellationToken);
    }
}