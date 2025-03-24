using UserService.Application.Validation.Exceptions.Base;

namespace UserService.Application.Validation.Exceptions.Specific;

public sealed class UserNotFoundByIdException : NotFoundException
{
    public UserNotFoundByIdException(Guid eventId)
        : base ($"The user with id: {eventId} does not exist in the database.")
    {
    }
}