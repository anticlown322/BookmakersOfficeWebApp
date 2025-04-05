using SportDataService.Application.DTO.Odds;

namespace SportDataService.Application.Contracts.UseCases.Odds;

public interface ICreateOddsUseCase
{
    Task<OddsGetDto> ExecuteAsync(OddsCreateDto oddsCreateDto, CancellationToken cancellationToken);
}