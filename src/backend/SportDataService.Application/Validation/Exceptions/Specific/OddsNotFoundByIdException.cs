using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class OddsNotFoundByIdException : NotFoundException
{
    public OddsNotFoundByIdException(string teamId)
        : base($"The odds with id: {teamId} does not exist in the database.")
    {
    }
}