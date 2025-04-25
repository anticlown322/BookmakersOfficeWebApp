namespace BettingService.BLL.DTO.Payout;

public record RequestPayoutDto(
    Guid BetId,
    decimal Amount);