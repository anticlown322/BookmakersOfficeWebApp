using System.Text.Json;
using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Contracts.Repository;
using BettingService.DAL.Models.Entities;
using BettingService.DAL.Models.Kafka.BetValidation;
using BettingService.DAL.Models.Settings.Kafka;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace BettingService.BLL.Services.Kafka;

public class ValidationAggregator : BackgroundService
{
    private readonly IKafkaConsumerService _consumerFactory;
    private readonly IServiceProvider _services;
    private readonly ILogger<ValidationAggregator> _logger;
    private readonly Dictionary<string, ValidationState> _validationStates = new();
    private readonly TimeSpan _validationTimeout = TimeSpan.FromMinutes(5);

    public ValidationAggregator(
        IKafkaConsumerService consumerFactory,
        IServiceProvider services,
        ILogger<ValidationAggregator> logger)
    {
        _consumerFactory = consumerFactory;
        _services = services;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        var userConsumer = _consumerFactory.CreateConsumer();
        userConsumer.Subscribe("user_validation_results");

        var sportConsumer = _consumerFactory.CreateConsumer();
        sportConsumer.Subscribe("sport_validation_results");

        // Process messages from both consumers in parallel
        var userTask = ProcessMessages(userConsumer, "user", ct);
        var sportTask = ProcessMessages(sportConsumer, "sport", ct);

        await Task.WhenAll(userTask, sportTask);
    }

    private async Task ProcessMessages(
        IConsumer<string, string> consumer,
        string validationType,
        CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(ct);
                    if (consumeResult == null) continue;

                    _logger.LogInformation("Received {Type} validation result for bet {BetId}", 
                        validationType, consumeResult.Message.Key);

                    using var scope = _services.CreateScope();
                    var betRepository = scope.ServiceProvider.GetRequiredService<IBetRepository>();
                    var producer = scope.ServiceProvider.GetRequiredService<IKafkaProducerService>();
                    var kafkaSettings = scope.ServiceProvider.GetRequiredService<IOptions<KafkaSettings>>().Value;

                    var betId = consumeResult.Message.Key;
                    var message = consumeResult.Message.Value;

                    if (!_validationStates.TryGetValue(betId, out var validationState))
                    {
                        validationState = new ValidationState();
                        _validationStates.Add(betId, validationState);
                    }

                    if (validationType == "user")
                    {
                        var userValidation = JsonSerializer.Deserialize<UserValidationResult>(message);
                        validationState.UserValidation = userValidation;
                    }
                    else
                    {
                        var sportValidation = JsonSerializer.Deserialize<SportValidationResult>(message);
                        validationState.SportValidation = sportValidation;
                    }

                    // Check if we have both validation results
                    if (validationState.UserValidation != null && validationState.SportValidation != null)
                    {
                        await ProcessCompleteValidation(betId, validationState, betRepository, producer, kafkaSettings, ct);
                        _validationStates.Remove(betId);
                    }

                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming {Type} validation message", validationType);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error processing {Type} validation", validationType);
                }
            }
        }
        finally
        {
            consumer.Close();
            consumer.Dispose();
        }
    }

    private async Task ProcessCompleteValidation(
        string betId,
        ValidationState validationState,
        IBetRepository betRepository,
        IKafkaProducerService producer,
        KafkaSettings kafkaSettings,
        CancellationToken ct)
    {
        var bet = await betRepository.GetByIdAsync(Guid.Parse(betId), ct);
        if (bet == null)
        {
            _logger.LogError("Bet {BetId} not found during validation aggregation", betId);
            return;
        }

        // Update bet status based on validation results
        if (!validationState.UserValidation.IsValid)
        {
            bet.Status = BetStatus.Rejected;
            bet.RejectionReason = validationState.UserValidation.RejectionReason;
            _logger.LogWarning("Bet {BetId} rejected by user validation: {Reason}", 
                betId, validationState.UserValidation.RejectionReason);
        }
        else if (!validationState.SportValidation.IsValid)
        {
            bet.Status = BetStatus.Rejected;
            bet.RejectionReason = validationState.SportValidation.RejectionReason;
            _logger.LogWarning("Bet {BetId} rejected by sport validation: {Reason}", 
                betId, validationState.SportValidation.RejectionReason);
        }
        else
        {
            bet.Status = BetStatus.Pending;
            bet.AcceptedAt = DateTime.UtcNow;
            bet.Odds = (decimal)validationState.SportValidation.CurrentOdds;
            _logger.LogInformation("Bet {BetId} validated successfully", betId);
        }

        betRepository.Update(bet);

        // Publish status update
        await producer.ProduceAsync(
            kafkaSettings.Topics.BetStatusUpdates,
            betId,
            new BetStatusUpdatedEvent
            {
                BetId = betId,
                NewStatus = bet.Status.ToString(),
                UpdatedOdds = (double)bet.Odds,
                Timestamp = DateTime.UtcNow
            },
            ct);
    }
}