using AutoMapper;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Tournament;

public class GetTournamentByTournamentIdUseCase(
    ITournamentRepository tournamentRepository,
    IMapper mapper)
    : IGetTournamentByTournamentIdUseCase
{
    public async Task<TournamentGetDto> ExecuteAsync(string tournamentId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tournament = await tournamentRepository.GetTournamentByTournamentIdAsync(tournamentId, cancellationToken);
        if (tournament == null)
        {
            throw new TournamentNotFoundByTournamentIdException(tournamentId);
        }

        return mapper.Map<TournamentGetDto>(tournament);
    }
}