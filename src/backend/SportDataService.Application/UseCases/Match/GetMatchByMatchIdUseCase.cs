using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.Validation.Exceptions.Specific;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Application.UseCases.Match;

public class GetMatchByMatchIdUseCase(
    IMatchRepository matchRepository,
    IMapper mapper,
    ILogger<GetMatchByMatchIdUseCase> logger)
    : IGetMatchByMatchIdUseCase
{
    public async Task<MatchGetDto> ExecuteAsync(string matchId, CancellationToken cancellationToken)
    {
        logger.LogInformation($"Getting match with mathc id {matchId}");

        cancellationToken.ThrowIfCancellationRequested();

        var match = await matchRepository.GetMatchByMatchIdAsync(matchId, cancellationToken);
        if (match == null)
        {
            logger.LogWarning($"Match with match id {matchId} not found");

            throw new MatchNotFoundByMatchIdException(matchId);
        }

        var result = mapper.Map<MatchGetDto>(match);

        logger.LogInformation($"Match with match id {matchId} successfully retrieved");

        return result;
    }
}