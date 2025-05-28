using AutoMapper;
using Microsoft.Extensions.Logging;
using SportDataService.Application.Contracts.UseCases.Tournament;
using SportDataService.Application.DTO.Prematch;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.UseCases.Tournament;

public sealed class GetAllTournamentsUseCase(
    ITournamentRepository tournamentRepository,
    IMapper mapper,
    ILogger<GetAllTournamentsUseCase> logger)
    : IGetAllTournamentsUseCase
{
    public async Task<(IEnumerable<TournamentGetDto> tournaments, MetaData metaData)> ExecuteAsync(
        TournamentParameters tournamentParameters,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all tournaments...");

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentsWithMetaData =
            await tournamentRepository.FindAllTournamentsAsync(tournamentParameters, cancellationToken);

        var tournamentGetDtos = mapper.Map<IEnumerable<TournamentGetDto>>(tournamentsWithMetaData);

        logger.LogInformation($"Successfully retrieved {tournamentGetDtos.Count()} tournaments");

        return (
            tournaments: tournamentGetDtos,
            metaData: tournamentsWithMetaData.MetaData);
    }
}