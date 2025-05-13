using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Results;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Infrastructure.Repository;

public sealed class MatchResultRepository : BaseCachedRepository<MatchResult>, IMatchResultRepository
{
    public MatchResultRepository(
        ICacheService cache,
        IOptions<CacheSettings> cacheSettings,
        IMongoDatabase database)
        : base(cache, cacheSettings, database, "matchResults")
    {
    }

    public async Task<PagedList<MatchResult>> FindAllMatchResultsAsync(
        MatchResultParameters matchResultParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cacheKey = $"match_results_{matchResultParameters.PageNumber}_{matchResultParameters.PageSize}";

        var cached = await Cache.GetAsync<PagedList<MatchResult>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var matchResults = await FindAllAsync(cancellationToken);
        var orderedMatchResults = matchResults.OrderBy(p => p.ResultTime);

        var pagedMatchResults = orderedMatchResults
            .Skip((matchResultParameters.PageNumber - 1) * matchResultParameters.PageSize)
            .Take(matchResultParameters.PageSize)
            .ToList();

        var totalCount = orderedMatchResults.Count();

        var result = new PagedList<MatchResult>(
            pagedMatchResults,
            totalCount,
            matchResultParameters.PageNumber,
            matchResultParameters.PageSize);

        if (totalCount > 0)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                result,
                DefaultCacheExpiration);
        }

        return result;
    }

    public async Task<MatchResult?> GetMatchResultByMatchResultIdAsync(string matchResultId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var cacheKey = $"match_result_{matchResultId}";

        var cached = await Cache.GetAsync<MatchResult>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        ct.ThrowIfCancellationRequested();

        var filter = Builders<MatchResult>.Filter.Eq(t => t.MatchResultId, matchResultId);

        var matchResult = await Collection.Find(filter).FirstOrDefaultAsync(ct);
        if (matchResult != null)
        {
            ct.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                matchResult,
                DefaultCacheExpiration);
        }

        return matchResult;
    }

    public override async Task UpdateAsync(MatchResult entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.UpdateAsync(entity, ct);

        var cacheKey = $"match_result_{entity.MatchResultId}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("match_results_");
    }

    public override async Task DeleteAsync(string id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.DeleteAsync(id, ct);

        var cacheKey = $"match_result_{id}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("match_results_");
    }
}