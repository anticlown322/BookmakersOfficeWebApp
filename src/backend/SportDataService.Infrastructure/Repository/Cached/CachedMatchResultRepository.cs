using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository.Cached;

public sealed class CachedMatchResultRepository(
    IMatchResultRepository decorated,
    ICacheService cache,
    IOptions<CacheSettings> cacheSettings)
    : IMatchResultRepository
{
    public async Task<PagedList<MatchResult>> FindAllMatchResultsAsync(
        MatchResultParameters matchResultParameters,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"match_results_{matchResultParameters.PageNumber}_{matchResultParameters.PageSize}";

        var cached = await cache.GetAsync<PagedList<MatchResult>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.FindAllMatchResultsAsync(matchResultParameters, cancellationToken);

        if (result.Count > 0)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public async Task<MatchResult?> GetMatchResultByMatchResultIdAsync(string matchResultId, CancellationToken ct)
    {
        var cacheKey = $"match_result_{matchResultId}";
        var cached = await cache.GetAsync<MatchResult>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.GetMatchResultByMatchResultIdAsync(matchResultId, ct);
        if (result != null)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public Task<IEnumerable<MatchResult>> FindAllAsync(CancellationToken ct)
        => decorated.FindAllAsync(ct);

    public Task<IEnumerable<MatchResult>> FindByConditionAsync(
        Expression<Func<MatchResult, bool>> predicate,
        CancellationToken ct)
        => decorated.FindByConditionAsync(predicate, ct);

    public Task<MatchResult?> GetByIdAsync(string id, CancellationToken ct)
        => decorated.GetByIdAsync(id, ct);

    public Task CreateAsync(MatchResult entity, CancellationToken ct)
        => decorated.CreateAsync(entity, ct);

    public async Task UpdateAsync(MatchResult entity, CancellationToken ct)
    {
        await decorated.UpdateAsync(entity, ct);

        var cacheKey = $"match_result_{entity.MatchResultId}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("match_results_");
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        await decorated.DeleteAsync(id, ct);

        var cacheKey = $"match_result_{id}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("match_results_");
    }
}