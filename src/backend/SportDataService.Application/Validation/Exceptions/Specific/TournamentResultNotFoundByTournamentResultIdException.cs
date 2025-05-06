using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TournamentResultNotFoundByTournamentResultIdException : NotFoundException
{
    public TournamentResultNotFoundByTournamentResultIdException(string tournamentId)
        : base($"The tournament result with tournamentId: {tournamentId} does not exist in the database.")
    {
    }
}