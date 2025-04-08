using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class TeamNotFoundByIdException : NotFoundException
{
    public TeamNotFoundByIdException(string id)
        : base($"The team with id: {id} does not exist in the database.")
    {
    }
}