using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.MatchResult;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.MatchResult;

public sealed class GetAllMatchResultsUseCase(
    IMatchResultRepository matchResultRepository,
    IMapper mapper,
    ILogger<GetAllMatchResultsUseCase> logger)
    : IGetAllMatchResultsUseCase
{
    public async Task<(IEnumerable<MatchResultGetDto> matchResults, MetaData metaData)> ExecuteAsync(
        MatchResultParameters matchResultParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting match results...");

        cancellationToken.ThrowIfCancellationRequested();

        var matchResultsWithMetaData =
            await matchResultRepository.FindAllMatchResultsAsync(matchResultParameters, cancellationToken);

        var matchResultGetDtos = mapper.Map<IEnumerable<MatchResultGetDto>>(matchResultsWithMetaData);

        logger.LogInformation("Match results successfully retrieved");

        return (
            matchResults: matchResultGetDtos,
            metaData: matchResultsWithMetaData.MetaData);
    }
}