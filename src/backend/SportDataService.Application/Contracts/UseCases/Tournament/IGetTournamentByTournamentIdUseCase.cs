using SportDataService.Application.DTO.Tournament;

namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IGetTournamentByTournamentIdUseCase
{
    Task<TournamentGetDto> ExecuteAsync(string tournamentId, CancellationToken cancellationToken);
}