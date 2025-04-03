using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.DTO.Player;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Player;

public sealed class GetPlayerByIdUseCase(
    IPlayerRepository playerRepository,
    IMapper mapper)
    : IGetPlayerByIdUseCase
{
    public async Task<PlayerGetDto> ExecuteAsync(string playerId, CancellationToken cancellationToken)
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

        return mapper.Map<PlayerGetDto>(player);
    }
}