using SportDataService.Application.DTO.Match;
using SportDataService.Application.DTO.Player;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.Player;

public interface IGetAllPlayersUseCase
{
    Task<(IEnumerable<PlayerGetDto> players, MetaData metaData)> ExecuteAsync(PlayerParameters playerParameters, CancellationToken cancellationToken);
}