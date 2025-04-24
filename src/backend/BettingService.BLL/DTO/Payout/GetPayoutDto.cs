using BettingService.DAL.Models.Entities;

namespace BettingService.BLL.DTO.Payout;

public record GetPayoutDto(
    Guid Id,
    Guid BetId,
    decimal Amount,
    PayoutStatus Status,
    DateTime ProcessedAt,
    string? ErrorReason);