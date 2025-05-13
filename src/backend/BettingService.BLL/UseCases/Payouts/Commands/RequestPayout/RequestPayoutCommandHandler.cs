using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Bets;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.MessageBroker;
using BettingService.DAL.Models.Settings.Kafka;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;

public sealed class PlacePayoutCommandHandler(
    IPayoutRepository payoutRepository,
    IBetRepository betRepository,
    IKafkaProducerService kafkaProducer,
    IKafkaConsumerService kafkaConsumer,
    IOptions<KafkaSettings> kafkaSettings)
    : IRequestHandler<RequestPayoutCommand, Unit>
{
    private readonly KafkaSettings _kafkaSettings = kafkaSettings.Value;

    public async Task<Unit> Handle(RequestPayoutCommand request, CancellationToken cancellationToken)
    {
        var bet = await betRepository.GetByIdAsync(request.RequestPayoutDto.BetId, cancellationToken);
        if (bet is null)
        {
            throw new BetNotFoundByIdException(request.RequestPayoutDto.BetId);
        }

        if (bet.Status != BetStatus.Won)
        {
            throw new InvalidBetStatusForPayoutException(bet.Id, bet.Status);
        }

        var existingPayout = await payoutRepository.GetByBetIdAsync(request.RequestPayoutDto.BetId, cancellationToken);
        if (existingPayout is not null &&
            (existingPayout.Status == PayoutStatus.Pending || existingPayout.Status == PayoutStatus.Completed))
        {
            throw new PayoutAlreadyExistsException(request.RequestPayoutDto.BetId, existingPayout.Status);
        }

        var expectedAmount = Math.Round(bet.Amount * bet.Odds, BetConstants.CalculationScale);
        var roundedRequested = Math.Round(request.RequestPayoutDto.Amount, BetConstants.CalculationScale);

        if (expectedAmount != roundedRequested)
        {
            throw new InvalidAmountForPayoutException(bet.Id, request.RequestPayoutDto.Amount);
        }

        var payout = new Payout
        {
            Id = Guid.NewGuid(),
            BetId = request.RequestPayoutDto.BetId,
            Username = request.Username,
            Amount = request.RequestPayoutDto.Amount,
            Status = PayoutStatus.Pending,
            CreatedAt = DateTime.UtcNow.ToUniversalTime(),
        };

        payoutRepository.Create(payout);
        await payoutRepository.SaveAsync(cancellationToken);

        var payoutRequest = new PayoutRequest
        {
            PayoutId = payout.Id.ToString(),
            CorrelationId = Guid.NewGuid().ToString(),
            BetId = bet.Id.ToString(),
            Username = request.Username,
            Amount = request.RequestPayoutDto.Amount,
        };

        await kafkaProducer.ProduceAsync(
            _kafkaSettings.Topics.PayoutRequests,
            payoutRequest,
            cancellationToken);

        var payoutResult = await WaitForPayoutResult(
            payoutRequest.CorrelationId,
            TimeSpan.FromSeconds(_kafkaSettings.RequestTimeoutSeconds),
            cancellationToken);

        if (payoutResult.IsSuccess)
        {
            payout.Status = PayoutStatus.Completed;
            payout.ProcessedAt = DateTime.UtcNow.ToUniversalTime();
        }
        else
        {
            payout.Status = PayoutStatus.Failed;
            payout.ErrorReason = payoutResult.ErrorMessage;
        }

        await payoutRepository.SaveAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<PayoutResult> WaitForPayoutResult(
        string correlationId,
        TimeSpan timeout,
        CancellationToken cancellationToken)
    {
        var startTime = DateTime.UtcNow;

        while (DateTime.UtcNow - startTime < timeout)
        {
            var message = await kafkaConsumer.ConsumeSingleMessageAsync<PayoutResult>(
                _kafkaSettings.Topics.PayoutResults,
                TimeSpan.FromSeconds(_kafkaSettings.ConsumeMsgTimeoutSeconds),
                cancellationToken);

            if (message != null && message.CorrelationId == correlationId)
            {
                return message;
            }
        }

        throw new TimeoutException($"Payout result not received within {timeout.TotalMinutes} minutes");
    }
}