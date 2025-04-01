using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class TokenNotCreatedException : UnauthorizedException
{
    public TokenNotCreatedException(string token)
        : base($"Cannot create access or refresh token {token}.")
    {
    }
}