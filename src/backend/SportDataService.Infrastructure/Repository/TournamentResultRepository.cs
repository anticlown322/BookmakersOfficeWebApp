using MongoDB.Driver;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository;

public class TournamentResultsRepository : MongoRepositoryBase<TournamentResult>, ITournamentResultRepository
{
    public TournamentResultsRepository(IMongoDatabase database)
        : base(database, "tournamentResults")
    {
    }

    public async Task<PagedList<TournamentResult>> FindAllTournamentResultsAsync(TournamentResultParameters tournamentResultParameters, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResults = await FindAllAsync(cancellationToken);

        var orderedTournamentResults = tournamentResults.OrderBy(p => p.TournamentName);

        var pagedTournamentResults = orderedTournamentResults
            .Skip((tournamentResultParameters.PageNumber - 1) * tournamentResultParameters.PageSize)
            .Take(tournamentResultParameters.PageSize)
            .ToList();

        var totalCount = orderedTournamentResults.Count();

        return new PagedList<TournamentResult>(
            pagedTournamentResults,
            totalCount,
            tournamentResultParameters.PageNumber,
            tournamentResultParameters.PageSize);
    }

    public async Task<TournamentResult?> GetTournamentResultByTournamentResultIdAsync(string tournamentResultId, CancellationToken ct)
    {
        var filter = Builders<TournamentResult>.Filter.Eq(t => t.TournamentId, tournamentResultId);

        ct.ThrowIfCancellationRequested();

        return await Collection.Find(filter).FirstOrDefaultAsync(ct);
    }
}