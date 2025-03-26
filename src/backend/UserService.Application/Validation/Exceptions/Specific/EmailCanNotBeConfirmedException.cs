using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class EmailCanNotBeConfirmedException : BadRequestException
{
    public EmailCanNotBeConfirmedException(string errorDescription)
        : base($"Email can not be confirmed. Reason: {errorDescription}.")
    {
    }
}