using SportDataService.Application.DTO.Prematch;

namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IGetTournamentByTournamentIdUseCase
{
    Task<TournamentGetDto> ExecuteAsync(string tournamentId, CancellationToken cancellationToken);
}