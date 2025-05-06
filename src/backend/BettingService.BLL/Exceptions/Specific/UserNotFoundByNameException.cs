using BettingService.BLL.Exceptions.Base;

namespace BettingService.BLL.Exceptions.Specific;

public sealed class UserNotFoundByNameException : NotFoundException
{
    public UserNotFoundByNameException(string username)
        : base($"The user with name: {username} does not exist in the database.")
    {
    }
}