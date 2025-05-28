using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Tournament;

public sealed class GetTournamentByIdUseCase(
    ITournamentRepository tournamentRepository,
    IMapper mapper,
    ILogger<GetTournamentByIdUseCase> logger)
    : IGetTournamentByIdUseCase
{
    public async Task<TournamentGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting tournament with id {id}...");

        if (!ObjectId.TryParse(id, out _))
        {
            logger.LogWarning($"Invalid id {id}");

            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentToGet = await tournamentRepository.GetByIdAsync(id, cancellationToken);
        if (tournamentToGet == null)
        {
            logger.LogWarning($"Tournament with id {id} not found");

            throw new TournamentNotFoundByIdException(id);
        }

        var result = mapper.Map<TournamentGetDto>(tournamentToGet);

        logger.LogInformation($"Successfully retrieved tournament with id {id}");

        return result;
    }
}