using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TournamentNotFoundByTournamentIdException : NotFoundException
{
    public TournamentNotFoundByTournamentIdException(string tournamentId)
        : base($"The tournament with tournamentId: {tournamentId} does not exist in the database.")
    {
    }
}