using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface ICreateMatchUseCase
{
    Task<MatchGetDto> ExecuteAsync(MatchCreateDto matchCreateDto, CancellationToken cancellationToken);
}