using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class MatchResultNotFoundByIdException : NotFoundException
{
    public MatchResultNotFoundByIdException(string id)
        : base($"The match result with id: {id} does not exist in the database.")
    {
    }
}