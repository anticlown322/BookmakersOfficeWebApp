using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Player;
using SportDataService.Application.DTO.Player;
using SportDataService.Application.DTO.Player;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Player;

public sealed class GetAllPlayersUseCase(
    IPlayerRepository playerRepository,
    IMapper mapper)
    : IGetAllPlayersUseCase
{
    public async Task<(IEnumerable<PlayerGetDto> players, MetaData metaData)> ExecuteAsync(PlayerParameters playerParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var playersWithMetaData = await playerRepository.FindAllPlayersAsync(playerParameters, cancellationToken);

        var playerGetDtos = mapper.Map<IEnumerable<PlayerGetDto>>(playersWithMetaData);

        return (
            players: playerGetDtos,
            metaData: playersWithMetaData.MetaData);
    }
}