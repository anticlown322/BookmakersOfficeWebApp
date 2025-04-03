using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class MatchNotFoundByIdException : NotFoundException
{
    public MatchNotFoundByIdException(string leagueId)
        : base($"The league with id: {leagueId} does not exist in the database.")
    {
    }
}