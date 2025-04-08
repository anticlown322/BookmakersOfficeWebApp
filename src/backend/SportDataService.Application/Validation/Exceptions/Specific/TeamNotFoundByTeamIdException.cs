using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TeamNotFoundByTeamIdException : NotFoundException
{
    public TeamNotFoundByTeamIdException(string teamId)
        : base($"The team with teamId: {teamId} does not exist in the database.")
    {
    }
}