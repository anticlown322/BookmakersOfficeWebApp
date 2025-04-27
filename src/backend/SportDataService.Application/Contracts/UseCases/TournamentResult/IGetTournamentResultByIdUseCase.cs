using SportDataService.Application.DTO.Results;

namespace SportDataService.Application.Contracts.UseCases.TournamentResult;

public interface IGetTournamentResultByIdUseCase
{
    Task<TournamentResultGetDto> ExecuteAsync(string id, CancellationToken cancellationToken);
}