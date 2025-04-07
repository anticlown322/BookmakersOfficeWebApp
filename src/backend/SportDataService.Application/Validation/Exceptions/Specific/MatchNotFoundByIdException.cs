using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class MatchNotFoundByIdException : NotFoundException
{
    public MatchNotFoundByIdException(string teamId)
        : base($"The match with id: {teamId} does not exist in the database.")
    {
    }
}