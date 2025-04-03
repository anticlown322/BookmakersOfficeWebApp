using SportDataService.Application.Validation.Exceptions.Base;

namespace SportDataService.Application.Validation.Exceptions.Specific;

public sealed class PlayerNotFoundByIdException : NotFoundException
{
    public PlayerNotFoundByIdException(string playerId)
        : base($"The player with id: {playerId} does not exist in the database.")
    {
    }
}