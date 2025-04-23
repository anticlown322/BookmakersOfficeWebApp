using BettingService.BLL.Exceptions.Base;
using BettingService.DAL.Models.Entities;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class InvalidBetStatusForPayoutException(Guid betId, BetStatus betStatus)
    : BadRequestException($"Bet with id {betId} has status {nameof(betStatus)}. For payout it must has status {nameof(BetStatus.Won)}.");