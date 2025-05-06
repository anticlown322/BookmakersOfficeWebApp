using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class MatchResultNotFoundByMatchResultIdException : NotFoundException
{
    public MatchResultNotFoundByMatchResultIdException(string matchId)
        : base($"The match result with matchId: {matchId} does not exist in the database.")
    {
    }
}