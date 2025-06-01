using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Infrastructure.Repository;

public class TournamentResultsRepository : BaseCachedRepository<TournamentResult>, ITournamentResultRepository
{
    public TournamentResultsRepository(
        ICacheService cache,
        IOptions<CacheSettings> cacheSettings,
        IMongoDatabase database)
        : base(cache, cacheSettings, database, "tournamentResults")
    {
    }

    public async Task<PagedList<TournamentResult>> FindAllTournamentResultsAsync(
        TournamentResultParameters tournamentResultParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cacheKey =
            $"tournament_results_{tournamentResultParameters.PageNumber}_{tournamentResultParameters.PageSize}";

        var cached = await Cache.GetAsync<PagedList<TournamentResult>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var tournamentResults = await FindAllAsync(cancellationToken);
        var orderedTournamentResults = tournamentResults.OrderBy(p => p.TournamentName);

        var pagedTournamentResults = orderedTournamentResults
            .Skip((tournamentResultParameters.PageNumber - 1) * tournamentResultParameters.PageSize)
            .Take(tournamentResultParameters.PageSize)
            .ToList();

        var totalCount = orderedTournamentResults.Count();

        var result = new PagedList<TournamentResult>(
            pagedTournamentResults,
            totalCount,
            tournamentResultParameters.PageNumber,
            tournamentResultParameters.PageSize);

        if (result.Count > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                result,
                DefaultCacheExpiration);
        }

        return result;
    }

    public async Task<TournamentResult?> GetTournamentResultByTournamentResultIdAsync(
        string tournamentResultId,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var cacheKey = $"tournament_result_{tournamentResultId}";

        var cached = await Cache.GetAsync<TournamentResult>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var filter = Builders<TournamentResult>.Filter.Eq(t => t.TournamentId, tournamentResultId);

        ct.ThrowIfCancellationRequested();

        var tournamentResult = await Collection.Find(filter).FirstOrDefaultAsync(ct);
        if (tournamentResult != null)
        {
            await Cache.SetAsync(
                cacheKey,
                tournamentResult,
                DefaultCacheExpiration);
        }

        return tournamentResult;
    }

    public override async Task UpdateAsync(TournamentResult entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.UpdateAsync(entity, ct);

        var cacheKey = $"tournament_result_{entity.TournamentId}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("tournament_results_");
    }

    public override async Task DeleteAsync(string id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.DeleteAsync(id, ct);

        var cacheKey = $"tournament_result_{id}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("tournament_results_");
    }
}