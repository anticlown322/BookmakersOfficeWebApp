using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Infrastructure.Repository;

public class TournamentsRepository : BaseCachedRepository<Tournament>, ITournamentRepository
{
    public TournamentsRepository(
        ICacheService cache,
        IOptions<CacheSettings> cacheSettings,
        IMongoDatabase database)
        : base(cache, cacheSettings, database, "tournaments")
    {
    }

    public async Task<PagedList<Tournament>> FindAllTournamentsAsync(
        TournamentParameters tournamentParameters,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var cacheKey = $"tournaments_{tournamentParameters.PageNumber}_{tournamentParameters.PageSize}";

        var cached = await Cache.GetAsync<PagedList<Tournament>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        ct.ThrowIfCancellationRequested();

        var tournaments = await FindAllAsync(ct);
        var orderedTournaments = tournaments.OrderBy(p => p.Name);

        var pagedTournaments = orderedTournaments
            .Skip((tournamentParameters.PageNumber - 1) * tournamentParameters.PageSize)
            .Take(tournamentParameters.PageSize)
            .ToList();

        var totalCount = orderedTournaments.Count();

        var result = new PagedList<Tournament>(
            pagedTournaments,
            totalCount,
            tournamentParameters.PageNumber,
            tournamentParameters.PageSize);

        if (result.Count > 0)
        {
            ct.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                result,
                DefaultCacheExpiration);
        }

        return result;
    }

    public async Task<Tournament?> GetTournamentByTournamentIdAsync(string tournamentId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        var cacheKey = $"tournament_{tournamentId}";

        var cached = await Cache.GetAsync<Tournament>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        ct.ThrowIfCancellationRequested();

        var filter = Builders<Tournament>.Filter.Eq(t => t.TournamentId, tournamentId);

        var result = await Collection.Find(filter).FirstOrDefaultAsync(ct);
        if (result != null)
        {
            ct.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                result,
                DefaultCacheExpiration);
        }

        return result;
    }

    public override async Task UpdateAsync(Tournament entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.UpdateAsync(entity, ct);

        var cacheKey = $"tournament_{entity.TournamentId}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("tournaments_");
    }

    public override async Task DeleteAsync(string id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.DeleteAsync(id, ct);

        var cacheKey = $"tournament_{id}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("tournaments_");
    }
}