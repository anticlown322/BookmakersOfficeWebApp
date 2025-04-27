using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TournamentResultNotFoundByIdException : NotFoundException
{
    public TournamentResultNotFoundByIdException(string id)
        : base($"The tournament result with id: {id} does not exist in the database.")
    {
    }
}