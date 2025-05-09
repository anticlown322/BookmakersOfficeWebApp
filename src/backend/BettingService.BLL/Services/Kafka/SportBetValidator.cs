using System.Text.Json;
using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.Kafka.BetValidation;
using BettingService.DAL.Models.Settings.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Kafka;

public class SportBetValidator(
    IBetRepository repository,
    IKafkaProducerService producer,
    IOptions<KafkaSettings> settings,
    ILogger<SportBetValidator> logger)
    : IMessageHandler<SportValidationResult>
{
    private readonly KafkaSettings _settings = settings.Value;

    public async Task HandleAsync(string message, CancellationToken ct)
    {
        var validation = JsonSerializer.Deserialize<SportValidationResult>(message);
        if (validation == null)
        {
            logger.LogError("Failed to deserialize sport validation message");
            return;
        }

        var bet = await repository.GetByIdAsync(Guid.Parse(validation.BetId), ct);
        if (bet == null)
        {
            logger.LogError("Bet {BetId} not found during sport validation", validation.BetId);
            return;
        }

        bet.Odds = (decimal)validation.CurrentOdds;

        if (!validation.IsValid)
        {
            bet.Status = BetStatus.Rejected;
            bet.RejectionReason = validation.RejectionReason;
            logger.LogWarning(
                "Sport validation failed for bet {BetId}: {Reason}",
                validation.BetId,
                validation.RejectionReason);
        }

        repository.Update(bet);
    }
}