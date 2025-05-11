using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Common;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;
using UserService.Domain.RequestFeatures;

namespace SportDataService.Infrastructure.Repository.Cached;

public sealed class CachedTeamRepository(
    ITeamRepository decorated,
    ICacheService cache,
    IOptions<CacheSettings> cacheSettings)
    : ITeamRepository
{
    public async Task<PagedList<Team>> FindAllTeamsAsync(
        TeamParameters teamParameters,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"teams_{teamParameters.PageNumber}_{teamParameters.PageSize}";

        var cached = await cache.GetAsync<PagedList<Team>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.FindAllTeamsAsync(teamParameters, cancellationToken);

        if (result.Count > 0)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public async Task<Team?> GetTeamByTeamIdAsync(string teamId, CancellationToken ct)
    {
        var cacheKey = $"team_{teamId}";

        var cached = await cache.GetAsync<Team>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.GetTeamByTeamIdAsync(teamId, ct);
        if (result != null)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public Task<IEnumerable<Team>> FindAllAsync(CancellationToken cancellationToken)
        => decorated.FindAllAsync(cancellationToken);

    public Task<IEnumerable<Team>> FindByConditionAsync(
        Expression<Func<Team, bool>> expression,
        CancellationToken ct) =>
        decorated.FindByConditionAsync(expression, ct);

    public Task<Team?> GetByIdAsync(string id, CancellationToken ct)
        => decorated.GetByIdAsync(id, ct);

    public Task CreateAsync(Team entity, CancellationToken ct) 
        => decorated.CreateAsync(entity, ct);

    public async Task UpdateAsync(Team entity, CancellationToken ct)
    {
        await decorated.UpdateAsync(entity, ct);

        var cacheKey = $"team_{entity.TeamId}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("teams_");
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        await decorated.DeleteAsync(id, ct);

        var cacheKey = $"team_{id}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("teams_");
    }
}