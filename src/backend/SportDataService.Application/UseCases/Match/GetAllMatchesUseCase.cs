using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.Match;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Match;

public sealed class GetAllMatchesUseCase(
    IMatchRepository matchRepository,
    IMapper mapper,
    ILogger<GetAllMatchesUseCase> logger)
    : IGetAllMatchesUseCase
{
    public async Task<(IEnumerable<MatchGetDto> matches, MetaData metaData)> ExecuteAsync(
        MatchParameters matchParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting matches...");

        cancellationToken.ThrowIfCancellationRequested();

        var matchesWithMetaData = await matchRepository.FindAllMatchesAsync(matchParameters, cancellationToken);

        var matchGetDtos = mapper.Map<IEnumerable<MatchGetDto>>(matchesWithMetaData);

        logger.LogInformation("Successfully retrieved matches");

        return (
            matches: matchGetDtos,
            metaData: matchesWithMetaData.MetaData);
    }
}