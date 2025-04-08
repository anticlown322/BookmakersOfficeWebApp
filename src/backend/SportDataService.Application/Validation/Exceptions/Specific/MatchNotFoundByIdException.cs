using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class MatchNotFoundByIdException : NotFoundException
{
    public MatchNotFoundByIdException(string id)
        : base($"The match with id: {id} does not exist in the database.")
    {
    }
}