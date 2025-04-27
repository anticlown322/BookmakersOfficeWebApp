using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ITournamentResultRepository : IRepositoryBase<TournamentResult>
{
    Task<PagedList<TournamentResult>> FindAllTournamentResultsAsync(TournamentResultParameters tournamentResultParameters, CancellationToken cancellationToken);
    Task<TournamentResult?> GetTournamentResultByTournamentResultIdAsync(string tournamentResultId, CancellationToken ct);
}