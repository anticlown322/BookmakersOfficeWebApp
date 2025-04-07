using SportDataService.Application.DTO.Tournament;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Application.Contracts.UseCases.Tournament;

public interface IGetAllTournamentsUseCase
{
    Task<(IEnumerable<TournamentGetDto> tournaments, MetaData metaData)> ExecuteAsync(TournamentParameters tournamentParameters, CancellationToken cancellationToken);
}