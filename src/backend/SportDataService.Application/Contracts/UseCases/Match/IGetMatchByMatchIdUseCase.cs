using SportDataService.Application.DTO.Prematch;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IGetMatchByMatchIdUseCase
{
    Task<MatchGetDto> ExecuteAsync(string matchId, CancellationToken cancellationToken);
}