using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class PasswordCanNotBeReset : BadRequestException
{
    public PasswordCanNotBeReset(string errorDescription)
        : base($"Password can not be reset. Reason: {errorDescription}.")
    {
    }
}