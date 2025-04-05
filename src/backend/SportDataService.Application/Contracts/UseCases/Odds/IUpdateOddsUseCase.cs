using SportDataService.Application.DTO.Odds;

namespace SportDataService.Application.Contracts.UseCases.Odds;

public interface IUpdateOddsUseCase
{
    Task ExecuteAsync(string oddsId, OddsUpdateDto oddsUpdateDto, CancellationToken cancellationToken);
}