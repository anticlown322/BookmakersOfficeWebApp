using MongoDB.Driver;
using SportDataService.Domain.Models.Tournaments;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public class TournamentsRepository : MongoRepositoryBase<Tournament>, ITournamentRepository
{
    public TournamentsRepository(IMongoDatabase database)
        : base(database, "tournaments")
    {
    }

    public async Task<PagedList<Tournament>> FindAllTournamentsAsync(TournamentParameters tournamentParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tournaments = await FindAllAsync(cancellationToken);

        var orderedTournaments = tournaments.OrderBy(p => p.Name);

        var pagedTournaments = orderedTournaments
            .Skip((tournamentParameters.PageNumber - 1) * tournamentParameters.PageSize)
            .Take(tournamentParameters.PageSize)
            .ToList();

        var totalCount = orderedTournaments.Count();

        return new PagedList<Tournament>(
            pagedTournaments,
            totalCount,
            tournamentParameters.PageNumber,
            tournamentParameters.PageSize);
    }

    public async Task<Tournament?> GetByTournamentIdAsync(string tournamentId, CancellationToken ct)
    {
        var filter = Builders<Tournament>.Filter.Eq(t => t.TournamentId, tournamentId);
        return await Collection.Find(filter).FirstOrDefaultAsync(ct);
    }
}