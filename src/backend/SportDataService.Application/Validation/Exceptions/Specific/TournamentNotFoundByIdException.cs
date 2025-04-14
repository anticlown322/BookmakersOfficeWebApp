using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TournamentNotFoundByIdException : NotFoundException
{
    public TournamentNotFoundByIdException(string id)
        : base($"The tournament with id: {id} does not exist in the database.")
    {
    }
}