using SportDataService.Application.DTO.Results;

namespace SportDataService.Application.Contracts.UseCases.TournamentResult;

public interface IGetTournamentResultByResultIdUseCase
{
    Task<TournamentResultGetDto> ExecuteAsync(string tournamentResultId, CancellationToken cancellationToken);
}