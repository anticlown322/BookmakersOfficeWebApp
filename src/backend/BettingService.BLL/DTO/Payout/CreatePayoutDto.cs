namespace BettingService.BLL.DTO.Payout;

public record CreatePayoutDto(
    Guid BetId,
    decimal Amount);