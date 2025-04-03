using SportDataService.Application.DTO.Player;

namespace SportDataService.Application.Contracts.UseCases.Player;

public interface ICreatePlayerUseCase
{
    Task<PlayerGetDto> ExecuteAsync(PlayerCreateDto playerCreateDto, CancellationToken cancellationToken);
}