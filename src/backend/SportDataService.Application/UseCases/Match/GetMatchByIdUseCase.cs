using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Match;

public sealed class GetMatchByIdUseCase(
    IMatchRepository matchRepository,
    IMapper mapper,
    ILogger<GetMatchByIdUseCase> logger)
    : IGetMatchByIdUseCase
{
    public async Task<MatchGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match by id {id}...");

        if (!ObjectId.TryParse(id, out _))
        {
            logger.LogWarning($"Invalid id {id}");

            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var match = await matchRepository.GetByIdAsync(id, cancellationToken);
        if (match == null)
        {
            logger.LogWarning($"Match with id {id} not found");

            throw new MatchNotFoundByIdException(id);
        }

        var result = mapper.Map<MatchGetDto>(match);

        logger.LogInformation($"Match with id {id} successfully retrieved");

        return result;
    }
}