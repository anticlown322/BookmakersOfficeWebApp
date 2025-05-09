namespace BettingService.DAL.Models.Kafka.BetValidation;

public record BetPlacementResult(Guid BetId, BetPlacementStatus Status);