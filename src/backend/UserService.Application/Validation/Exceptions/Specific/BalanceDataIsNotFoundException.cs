using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class BalanceDataIsNotFoundException : NotFoundException
{
    public BalanceDataIsNotFoundException(string username)
        : base($"Balance data is incorrect or balance can not be found for user {username}. ")
    {
    }
}