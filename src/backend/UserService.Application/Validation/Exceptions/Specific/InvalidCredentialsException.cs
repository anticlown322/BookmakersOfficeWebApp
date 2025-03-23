using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class InvalidCredentialsException : UnauthorizedException
{
    public InvalidCredentialsException(string name, string password)
        : base($"Cannot login with username {name} and password {password}.")
    {
    }
}