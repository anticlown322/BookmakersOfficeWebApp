using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Prematch;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Infrastructure.Repository;

public sealed class MatchRepository : BaseCachedRepository<Match>, IMatchRepository
{
    public MatchRepository(
        ICacheService cache,
        IOptions<CacheSettings> cacheSettings,
        IMongoDatabase database)
        : base(cache, cacheSettings, database, "matches")
    {
    }

    public async Task<PagedList<Match>> FindAllMatchesAsync(
        MatchParameters matchParameters,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var cacheKey = $"matches_{matchParameters.PageNumber}_{matchParameters.PageSize}";

        var cached = await Cache.GetAsync<PagedList<Match>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        cancellationToken.ThrowIfCancellationRequested();

        var matches = await FindAllAsync(cancellationToken);

        var orderedMatches = matches.OrderBy(p => p.StartTime);

        var pagedMatches = orderedMatches
            .Skip((matchParameters.PageNumber - 1) * matchParameters.PageSize)
            .Take(matchParameters.PageSize)
            .ToList();

        var totalCount = orderedMatches.Count();

        var result = new PagedList<Match>(
            pagedMatches,
            totalCount,
            matchParameters.PageNumber,
            matchParameters.PageSize);

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

    public async Task<Match?> GetMatchByMatchIdAsync(string matchId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var cacheKey = $"match_id_{matchId}";

        var cached = await Cache.GetAsync<Match>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        ct.ThrowIfCancellationRequested();

        var filter = Builders<Match>.Filter.Eq(t => t.MatchId, matchId);

        var match = await Collection.Find(filter).FirstOrDefaultAsync(ct);
        if (match != null)
        {
            ct.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                match,
                DefaultCacheExpiration);
        }

        return match;
    }

    public async Task<List<Match>> GetMatchesStartedBeforeAsync(DateTime cutoffTime, CancellationToken ct)
    {
        var matches = await FindByConditionAsync(m => m.StartTime != null && m.StartTime < cutoffTime, ct);
        var matchesList = matches.ToList();

        return matchesList;
    }

    public override async Task UpdateAsync(Match entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.UpdateAsync(entity, ct);

        var cacheKey = $"match_id_{entity.MatchId}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("matches_");
    }

    public override async Task DeleteAsync(string id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.DeleteAsync(id, ct);

        var matchCacheKey = $"match_id_{id}";
        await Cache.RemoveAsync(matchCacheKey);

        await Cache.RemoveByPrefixAsync("matches_");
    }
}