using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.TournamentResult;

public sealed class GetTournamentResultByIdUseCase(
    ITournamentResultRepository tournamentResultRepository,
    IMapper mapper,
    ILogger<GetTournamentResultByIdUseCase> logger)
    : IGetTournamentResultByIdUseCase
{
    public async Task<TournamentResultGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting tournament result with id {id}...");

        if (!ObjectId.TryParse(id, out _))
        {
            logger.LogWarning($"Invalid id {id}");

            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResultToGet = await tournamentResultRepository.GetByIdAsync(id, cancellationToken);
        if (tournamentResultToGet == null)
        {
            logger.LogWarning($"Tournament result with id {id} not found");

            throw new TournamentResultNotFoundByIdException(id);
        }

        var result = mapper.Map<TournamentResultGetDto>(tournamentResultToGet);

        logger.LogInformation($"Tournament result with {id} retrieved");

        return result;
    }
}