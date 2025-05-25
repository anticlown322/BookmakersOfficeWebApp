using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.MatchResult;

public class GetMatchResultByResultIdUseCase(
    IMatchResultRepository matchResultRepository,
    IMapper mapper,
    ILogger<GetMatchResultByResultIdUseCase> logger)
    : IGetMatchResultByResultIdUseCase
{
    public async Task<MatchResultGetDto> ExecuteAsync(string matchResultId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match result with match result id {matchResultId}");

        cancellationToken.ThrowIfCancellationRequested();

        var matchResult =
            await matchResultRepository.GetMatchResultByMatchResultIdAsync(matchResultId, cancellationToken);
        if (matchResult == null)
        {
            logger.LogWarning($"Match result with match result id {matchResultId} not found");

            throw new MatchResultNotFoundByMatchResultIdException(matchResultId);
        }

        var result = mapper.Map<MatchResultGetDto>(matchResult);

        logger.LogInformation($"Match result with match result id {matchResultId} successfully retrieved");

        return result;
    }
}