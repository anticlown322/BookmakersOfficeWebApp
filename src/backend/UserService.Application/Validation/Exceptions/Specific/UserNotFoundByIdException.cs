using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class UserNotFoundByIdException : NotFoundException
{
    public UserNotFoundByIdException(Guid userId)
        : base($"The user with id: {userId} does not exist in the database.")
    {
    }
}