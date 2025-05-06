using SportDataService.Application.DTO.Prematch;

namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IGetTournamentByIdUseCase
{
    Task<TournamentGetDto> ExecuteAsync(string id, CancellationToken cancellationToken);
}