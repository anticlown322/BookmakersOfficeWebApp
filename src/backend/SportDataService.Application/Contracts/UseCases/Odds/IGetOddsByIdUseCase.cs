using SportDataService.Application.DTO.Odds;

namespace SportDataService.Application.Contracts.UseCases.Odds;

public interface IGetOddsByIdUseCase
{
    Task<OddsGetDto> ExecuteAsync(string oddsId, CancellationToken cancellationToken);
}