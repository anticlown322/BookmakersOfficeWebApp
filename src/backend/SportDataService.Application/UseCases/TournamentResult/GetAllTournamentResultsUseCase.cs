using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.TournamentResult;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.TournamentResult;

public sealed class GetAllTournamentResultsUseCase(
    ITournamentResultRepository tournamentResultRepository,
    IMapper mapper,
    ILogger<GetAllTournamentResultsUseCase> logger)
    : IGetAllTournamentResultsUseCase
{
    public async Task<(IEnumerable<TournamentResultGetDto> tournamentResults, MetaData metaData)> ExecuteAsync(
        TournamentResultParameters tournamentResultParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting tournament results...");

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResultsWithMetaData =
            await tournamentResultRepository.FindAllTournamentResultsAsync(
                tournamentResultParameters,
                cancellationToken);

        var tournamentResultGetDtos = mapper.Map<IEnumerable<TournamentResultGetDto>>(tournamentResultsWithMetaData);

        logger.LogInformation("Tournament results retrieved");

        return (
            tournamentResults: tournamentResultGetDtos,
            metaData: tournamentResultsWithMetaData.MetaData);
    }
}