using BettingService.BLL.Contracts.Services;
using BettingService.BLL.Exceptions.Specific;
using BettingService.BLL.UseCases.Bets;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.MessageBroker;
using BettingService.DAL.Models.Settings.Kafka;
using BettingService.Protos;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.UseCases.Payouts.Commands.RequestPayout;

public sealed class PlacePayoutCommandHandler(
    IPayoutRepository payoutRepository,
    IBetRepository betRepository,
    UserGrpcService.UserGrpcServiceClient userClient,
    ILogger<PlacePayoutCommandHandler> logger,
    IKafkaProducerService kafkaProducer,
    IKafkaConsumerService kafkaConsumer,
    IOptions<KafkaSettings> kafkaSettings)
    : IRequestHandler<RequestPayoutCommand, Unit>
{
    private readonly KafkaSettings _kafkaSettings = kafkaSettings.Value;

    public async Task<Unit> Handle(RequestPayoutCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Checking bet with id {request.RequestPayoutDto.BetId} for payout...");

        var bet = await betRepository.GetByIdAsync(request.RequestPayoutDto.BetId, cancellationToken);
        if (bet is null)
        {
            logger.LogWarning($"Bet with id {request.RequestPayoutDto.BetId} not found");

            throw new BetNotFoundByIdException(request.RequestPayoutDto.BetId);
        }

        if (bet.Status != BetStatus.Won)
        {
            logger.LogWarning($"Bet status is {bet.Status} and doesn't equal to {BetStatus.Won}");

            throw new InvalidBetStatusForPayoutException(bet.Id, bet.Status);
        }

        logger.LogInformation("Checking for existing payout for bet...");

        var existingPayout = await payoutRepository.GetByBetIdAsync(request.RequestPayoutDto.BetId, cancellationToken);
        if (existingPayout is not null &&
            (existingPayout.Status == PayoutStatus.Pending || existingPayout.Status == PayoutStatus.Completed))
        {
            logger.LogWarning($"Payout for bet {request.RequestPayoutDto.BetId} already exists");

            throw new PayoutAlreadyExistsException(request.RequestPayoutDto.BetId, existingPayout.Status);
        }

        logger.LogInformation("Checking for user to be payed out...");

        var balanceResponse = await userClient.GetUserBalanceAsync(
            new GetUserBalanceRequest { Username = request.Username },
            cancellationToken: cancellationToken);
        if (!balanceResponse.UserExists)
        {
            logger.LogWarning($"User {request.Username} not found");

            throw new UserNotFoundByNameException(request.Username);
        }

        logger.LogInformation("Checking odds for payout...");

        var expectedAmount = Math.Round(bet.Amount * bet.Odds, BetConstants.CalculationScale);
        var roundedRequested = Math.Round(request.RequestPayoutDto.Amount, BetConstants.CalculationScale);

        if (expectedAmount != roundedRequested)
        {
            logger.LogWarning($"Expected {expectedAmount} but got {roundedRequested}");

            throw new InvalidAmountForPayoutException(bet.Id, request.RequestPayoutDto.Amount);
        }

        logger.LogInformation("Saving payout...");

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

        logger.LogInformation("Payout successfully placed and saved");

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