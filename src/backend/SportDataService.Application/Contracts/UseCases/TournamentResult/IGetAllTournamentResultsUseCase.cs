using SportDataService.Application.DTO.Prematch;
using SportDataService.Application.DTO.Results;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.TournamentResult;

public interface IGetAllTournamentResultsUseCase
{
    Task<(IEnumerable<TournamentResultGetDto> tournamentResults, MetaData metaData)> ExecuteAsync(
        TournamentResultParameters tournamentParameters,
        CancellationToken cancellationToken);
}