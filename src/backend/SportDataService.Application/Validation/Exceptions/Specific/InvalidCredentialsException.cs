using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class InvalidCredentialsException : UnauthorizedException
{
    public InvalidCredentialsException(string userId)
        : base($"User with id {userId} not authenticated.")
    {
    }
}