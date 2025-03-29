using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class InvalidBalanceWithdrawException : BadRequestException
{
    public InvalidBalanceWithdrawException(string errorDescription)
        : base($"Balance withdraw is canceled. Reason: {errorDescription}")
    {
    }
}