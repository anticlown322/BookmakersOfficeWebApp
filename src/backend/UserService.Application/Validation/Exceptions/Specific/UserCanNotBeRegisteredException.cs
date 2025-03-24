using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class UserCanNonBeRegistered : BadRequestException
{
    public UserCanNonBeRegistered(string errorDescription)
        : base($"User can't be registered. Reason: {errorDescription}.")
    {
    }
}