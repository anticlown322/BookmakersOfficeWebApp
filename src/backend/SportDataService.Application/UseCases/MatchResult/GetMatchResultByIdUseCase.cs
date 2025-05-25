using AutoMapper;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.MatchResult;

public sealed class GetMatchResultByIdUseCase(
    IMatchResultRepository matchResultRepository,
    IMapper mapper,
    ILogger<GetMatchResultByIdUseCase> logger)
    : IGetMatchResultByIdUseCase
{
    public async Task<MatchResultGetDto> ExecuteAsync(string id, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match result with id {id} successfully retrieved");

        if (!ObjectId.TryParse(id, out _))
        {
            logger.LogWarning($"Invalid id {id}");

            throw new InvalidIdFormatException(id);
        }

        cancellationToken.ThrowIfCancellationRequested();

        var matchResult = await matchResultRepository.GetByIdAsync(id, cancellationToken);
        if (matchResult == null)
        {
            logger.LogWarning($"Match result with id {id} not found");

            throw new MatchResultNotFoundByIdException(id);
        }

        var result = mapper.Map<MatchResultGetDto>(matchResult);

        logger.LogInformation($"Match result with id {id} successfully retrieved");

        return result;
    }
}