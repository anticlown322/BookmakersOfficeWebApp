using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TournamentNotFoundByIdException : NotFoundException
{
    public TournamentNotFoundByIdException(string tournamentId)
        : base($"The tournament with id: {tournamentId} does not exist in the database.")
    {
    }
}