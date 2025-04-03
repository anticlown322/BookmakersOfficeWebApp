using SportDataService.Application.DTO.Player;

namespace SportDataService.Application.Contracts.UseCases.Player;

public interface IUpdatePlayerUseCase
{
    Task ExecuteAsync(string playerId, PlayerUpdateDto playerUpdateDto, CancellationToken cancellationToken);
}