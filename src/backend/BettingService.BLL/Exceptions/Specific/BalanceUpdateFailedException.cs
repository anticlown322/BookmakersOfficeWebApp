using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class BalanceUpdateFailedException(string username)
    : BadRequestException($"Balance update failed for username {username}");