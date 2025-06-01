using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Domain.RepositoryContracts;

public interface ITournamentRepository : ICachedRepository<Tournament>
{
    Task<PagedList<Tournament>> FindAllTournamentsAsync(TournamentParameters tournamentParameters, CancellationToken ct);
    Task<Tournament?> GetTournamentByTournamentIdAsync(string tournamentId, CancellationToken ct);
}