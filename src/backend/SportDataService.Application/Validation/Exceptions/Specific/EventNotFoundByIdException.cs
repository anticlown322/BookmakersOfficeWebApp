using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class EventNotFoundByIdException : NotFoundException
{
    public EventNotFoundByIdException(string teamId)
        : base($"The team with id: {teamId} does not exist in the database.")
    {
    }
}