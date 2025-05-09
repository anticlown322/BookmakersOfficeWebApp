using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.Settings.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Kafka;

public class BetValidator(
    IKafkaProducerService producer,
    IOptions<KafkaSettings> settings,
    ILogger<BetValidator> logger)
{
    private readonly KafkaSettings _settings = settings.Value;

    public async Task ValidateNewBetAsync(Bet bet, CancellationToken ct)
    {
        await producer.ProduceAsync(
            _settings.Topics.UserValidationResults,
            bet.Id.ToString(),
            new { bet.Id, bet.Username, bet.Amount },
            ct);

        await producer.ProduceAsync(
            _settings.Topics.SportValidationResults,
            bet.Id.ToString(),
            new { bet.Id, bet.MatchId, bet.Odds },
            ct);

        logger.LogInformation("Validation requests sent for bet {BetId}", bet.Id);
    }
}