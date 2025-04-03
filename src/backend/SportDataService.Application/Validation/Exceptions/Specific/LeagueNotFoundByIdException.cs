using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class LeagueNotFoundByIdException : NotFoundException
{
    public LeagueNotFoundByIdException(string leagueId)
        : base($"The league with id: {leagueId} does not exist in the database.")
    {
    }
}