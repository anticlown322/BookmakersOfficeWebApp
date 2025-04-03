using AutoMapper;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.DTO.Player;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Player;

public sealed class CreatePlayerUseCase(
    IPlayerRepository playerRepository,
    IMapper mapper) : ICreatePlayerUseCase
{
    public async Task<PlayerGetDto> ExecuteAsync(PlayerCreateDto playerCreateDto, CancellationToken cancellationToken)
    {
        var player = mapper.Map<Domain.Models.Player>(playerCreateDto);
        player.Id = ObjectId.GenerateNewId().ToString();

        cancellationToken.ThrowIfCancellationRequested();

        await playerRepository.CreateAsync(player, cancellationToken);

        var playerGetDto = mapper.Map<PlayerGetDto>(player);
        return playerGetDto;
    }
}