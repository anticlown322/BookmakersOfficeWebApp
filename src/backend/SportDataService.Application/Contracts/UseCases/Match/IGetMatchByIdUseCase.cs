using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IGetMatchByIdUseCase
{
    Task<MatchGetDto> ExecuteAsync(string matchId, CancellationToken cancellationToken);
}