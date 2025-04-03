using SportDataService.Application.DTO.Player;

namespace SportDataService.Application.Contracts.UseCases.Player;

public interface IGetPlayerByIdUseCase
{
    Task<PlayerGetDto> ExecuteAsync(string playerId, CancellationToken cancellationToken);
}