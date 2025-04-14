using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IGetMatchByMatchIdUseCase
{
    Task<MatchGetDto> ExecuteAsync(string matchId, CancellationToken cancellationToken);
}