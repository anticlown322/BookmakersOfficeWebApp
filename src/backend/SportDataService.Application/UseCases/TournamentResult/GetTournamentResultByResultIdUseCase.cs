using AutoMapper;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.TournamentResult;

public class GetTournamentResultByResultIdUseCase(
    ITournamentResultRepository tournamentResultRepository,
    IMapper mapper)
    : IGetTournamentResultByResultIdUseCase
{
    public async Task<TournamentResultGetDto> ExecuteAsync(string tournamentResultId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResult = await tournamentResultRepository.GetTournamentResultByTournamentResultIdAsync(tournamentResultId, cancellationToken);
        if (tournamentResult == null)
        {
            throw new TournamentResultNotFoundByTournamentResultIdException(tournamentResultId);
        }

        return mapper.Map<TournamentResultGetDto>(tournamentResult);
    }
}