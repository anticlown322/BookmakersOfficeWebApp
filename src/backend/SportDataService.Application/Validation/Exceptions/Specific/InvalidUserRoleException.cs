using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class InvalidUserRoleException : UnauthorizedException
{
    public InvalidUserRoleException(string userRole)
        : base($"User with role {userRole} has no rights to access this resource.")
    {
    }
}