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

public sealed class CachedMatchRepository(
    IMatchRepository decorated,
    ICacheService cache,
    IOptions<CacheSettings> cacheSettings)
    : IMatchRepository
{
    public async Task<PagedList<Match>> FindAllMatchesAsync(
        MatchParameters matchParameters,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"matches_{matchParameters.PageNumber}_{matchParameters.PageSize}";

        var cached = await cache.GetAsync<PagedList<Match>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.FindAllMatchesAsync(matchParameters, cancellationToken);

        if (result.Capacity > 0)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public async Task<Match?> GetMatchByMatchIdAsync(string matchId, CancellationToken ct)
    {
        var cacheKey = $"match_id_{matchId}";
        var cached = await cache.GetAsync<Match>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.GetMatchByMatchIdAsync(matchId, ct);
        if (result != null)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public async Task<List<Match>> GetMatchesStartedBeforeAsync(DateTime cutoffTime, CancellationToken ct)
    {
        if (cutoffTime.Date == DateTime.UtcNow.Date)
        {
            var cacheKey = $"matches_before_{cutoffTime:yyyyMMdd}";
            var cached = await cache.GetAsync<List<Match>>(cacheKey);
            if (cached != null)
            {
                return cached;
            }

            var result = await decorated.GetMatchesStartedBeforeAsync(cutoffTime, ct);
            if (result.Any())
            {
                await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(30));
            }

            return result;
        }

        return await decorated.GetMatchesStartedBeforeAsync(cutoffTime, ct);
    }

    public Task<IEnumerable<Match>> FindAllAsync(CancellationToken cancellationToken)
        => decorated.FindAllAsync(cancellationToken);

    public Task<IEnumerable<Match>> FindByConditionAsync(
        Expression<Func<Match, bool>> expression,
        CancellationToken ct)
        => decorated.FindByConditionAsync(expression, ct);

    public Task<Match?> GetByIdAsync(string id, CancellationToken ct)
        => decorated.GetByIdAsync(id, ct);

    public Task CreateAsync(Match entity, CancellationToken ct)
        => decorated.CreateAsync(entity, ct);

    public async Task UpdateAsync(Match entity, CancellationToken ct)
    {
        await decorated.UpdateAsync(entity, ct);

        var cacheKey = $"match_id_{entity.MatchId}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("matches_");
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        await decorated.DeleteAsync(id, ct);

        var matchCacheKey = $"match_id_{id}";
        await cache.RemoveAsync(matchCacheKey);

        await cache.RemoveByPrefixAsync("matches_");
    }
}