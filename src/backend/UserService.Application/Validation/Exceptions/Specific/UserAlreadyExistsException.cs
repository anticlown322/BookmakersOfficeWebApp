using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class UserAlreadyExistsException : BadRequestException
{
    public UserAlreadyExistsException(string name)
        : base($"User with {name} username already exists.")
    {
    }
}