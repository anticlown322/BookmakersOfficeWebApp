using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class MatchNotFoundByMatchIdException : NotFoundException
{
    public MatchNotFoundByMatchIdException(string matchId)
        : base($"The match with matchId: {matchId} does not exist in the database.")
    {
    }
}