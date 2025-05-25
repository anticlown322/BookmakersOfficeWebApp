using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Tournament;

public class GetTournamentByTournamentIdUseCase(
    ITournamentRepository tournamentRepository,
    IMapper mapper,
    ILogger<GetTournamentByTournamentIdUseCase> logger)
    : IGetTournamentByTournamentIdUseCase
{
    public async Task<TournamentGetDto> ExecuteAsync(string tournamentId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting tournament with tournament id {tournamentId}");

        cancellationToken.ThrowIfCancellationRequested();

        var tournament = await tournamentRepository.GetTournamentByTournamentIdAsync(tournamentId, cancellationToken);
        if (tournament == null)
        {
            logger.LogWarning($"Tournament with tournament id {tournamentId} not found");

            throw new TournamentNotFoundByTournamentIdException(tournamentId);
        }

        var result = mapper.Map<TournamentGetDto>(tournament);

        logger.LogInformation($"Successfully retrieved tournament with tournament id {tournamentId}");

        return result;
    }
}