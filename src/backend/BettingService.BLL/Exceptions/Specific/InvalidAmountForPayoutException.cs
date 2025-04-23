using BettingService.BLL.Exceptions.Base;
using BettingService.DAL.Models.Entities;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class InvalidAmountForPayoutException(Guid betId, decimal amount)
    : UnauthorizedException($"Bet with id {betId} has amount that differs from requested value = {amount}.");