using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class InvalidCredentialsException(string userId)
    : UnauthorizedException($"User with id {userId} not authenticated.");