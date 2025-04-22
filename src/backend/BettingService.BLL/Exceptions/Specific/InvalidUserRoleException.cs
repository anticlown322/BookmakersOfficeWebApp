using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class InvalidUserRoleException(string userRole)
    : UnauthorizedException($"User with role {userRole} has no rights to access this resource.");