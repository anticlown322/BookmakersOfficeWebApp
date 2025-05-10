using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository.Cached;

public sealed class CachedTournamentRepository(
    ITournamentRepository decorated,
    ICacheService cache,
    IOptions<CacheSettings> cacheSettings)
    : ITournamentRepository
{
    public async Task<PagedList<Tournament>> FindAllTournamentsAsync(
        TournamentParameters tournamentParameters,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"tournaments_{tournamentParameters.PageNumber}_{tournamentParameters.PageSize}";

        var cached = await cache.GetAsync<PagedList<Tournament>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.FindAllTournamentsAsync(tournamentParameters, cancellationToken);

        if (result.Count > 0)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public async Task<Tournament?> GetTournamentByTournamentIdAsync(string tournamentId, CancellationToken ct)
    {
        var cacheKey = $"tournament_{tournamentId}";

        var cached = await cache.GetAsync<Tournament>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.GetTournamentByTournamentIdAsync(tournamentId, ct);
        if (result != null)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public Task<IEnumerable<Tournament>> FindAllAsync(CancellationToken cancellationToken)
    {
        return decorated.FindAllAsync(cancellationToken);
    }

    public Task<IEnumerable<Tournament>> FindByConditionAsync(
        Expression<Func<Tournament, bool>> expression,
        CancellationToken ct)
    {
        return decorated.FindByConditionAsync(expression, ct);
    }

    public Task<Tournament?> GetByIdAsync(string id, CancellationToken ct)
    {
        return decorated.GetByIdAsync(id, ct);
    }

    public Task CreateAsync(Tournament entity, CancellationToken ct)
    {
        return decorated.CreateAsync(entity, ct);
    }

    public async Task UpdateAsync(Tournament entity, CancellationToken ct)
    {
        await decorated.UpdateAsync(entity, ct);

        var cacheKey = $"tournament_{entity.TournamentId}";
        await cache.RemoveAsync(cacheKey);
        await cache.RemoveByPrefixAsync("tournaments_");
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        await decorated.DeleteAsync(id, ct);

        var cacheKey = $"tournament_{id}";
        await cache.RemoveAsync(cacheKey);
        await cache.RemoveByPrefixAsync("tournaments_");
    }
}