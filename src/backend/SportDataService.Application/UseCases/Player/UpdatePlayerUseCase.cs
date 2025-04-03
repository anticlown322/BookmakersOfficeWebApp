using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.DTO.Player;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Player;

public sealed class UpdatePlayerUseCase(
    IPlayerRepository playerRepository,
    IMapper mapper)
    : IUpdatePlayerUseCase
{
    public async Task ExecuteAsync(string playerId, PlayerUpdateDto playerUpdateDto, CancellationToken cancellationToken)
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

        mapper.Map(playerUpdateDto, player);
        await playerRepository.UpdateAsync(player, cancellationToken);
    }
}