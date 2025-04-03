using SportDataService.Application.DTO.Match;

namespace SportDataService.Application.Contracts.UseCases.Match;

public interface IUpdateMatchUseCase
{
    Task ExecuteAsync(string matchId, MatchUpdateDto matchUpdateDto, CancellationToken cancellationToken);
}