using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.TournamentResult;

public class GetTournamentResultByResultIdUseCase(
    ITournamentResultRepository tournamentResultRepository,
    IMapper mapper,
    ILogger<GetTournamentResultByResultIdUseCase> logger)
    : IGetTournamentResultByResultIdUseCase
{
    public async Task<TournamentResultGetDto> ExecuteAsync(
        string tournamentResultId,
        CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting tournament result with tournament result id {tournamentResultId}...");

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResult =
            await tournamentResultRepository.GetTournamentResultByTournamentResultIdAsync(
                tournamentResultId,
                cancellationToken);
        if (tournamentResult == null)
        {
            logger.LogWarning($"Tournament result with tournament result id {tournamentResult} not found");

            throw new TournamentResultNotFoundByTournamentResultIdException(tournamentResultId);
        }

        var result = mapper.Map<TournamentResultGetDto>(tournamentResult);

        logger.LogInformation(
            $"Tournament result with tournament result id {tournamentResultId} successfully retrieved");

        return result;
    }
}