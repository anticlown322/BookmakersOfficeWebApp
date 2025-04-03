using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Player;

public sealed class DeletePlayerUseCase(
    IPlayerRepository playerRepository)
    : IDeletePlayerUseCase
{
    public async Task ExecuteAsync(string playerId, CancellationToken cancellationToken)
    {
        if (!ObjectId.TryParse(playerId, out _))
        {
            throw new ArgumentException("Invalid Player ID format.");
        }

        var player = await playerRepository.GetByIdAsync(playerId, cancellationToken);
        if (player == null)
        {
            throw new PlayerNotFoundByIdException(playerId);
        }

        await playerRepository.DeleteAsync(playerId, cancellationToken);
    }
}