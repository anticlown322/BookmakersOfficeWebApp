using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class InsufficientFundsException(decimal amount)
    : BadRequestException($"Insufficient funds for amount {amount}.");