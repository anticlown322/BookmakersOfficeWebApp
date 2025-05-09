using System.Text.Json;
using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.Kafka.BetValidation;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.Services.Kafka;

public class UserBetValidator(
    IBetRepository repository,
    ILogger<UserBetValidator> logger)
    : IMessageHandler<UserValidationResult>
{
    public async Task HandleAsync(string message, CancellationToken ct)
    {
        var validation = JsonSerializer.Deserialize<UserValidationResult>(message);
        if (validation == null)
        {
            logger.LogError("Failed to deserialize user validation message");
            return;
        }

        var bet = await repository.GetByIdAsync(Guid.Parse(validation.BetId), ct);
        if (bet == null)
        {
            logger.LogError("Bet {BetId} not found during user validation", validation.BetId);
            return;
        }

        if (!validation.IsValid)
        {
            bet.Status = BetStatus.Rejected;
            bet.RejectionReason = validation.RejectionReason;
            logger.LogWarning("User validation failed for bet {BetId}: {Reason}", validation.BetId, validation.RejectionReason);
        }
    }
}