using Microsoft.Extensions.Options;
using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Common;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;
using SportDataService.Domain.RequestFeatures;
using SportDataService.Domain.RequestFeatures.Params;

namespace SportDataService.Infrastructure.Repository;

public sealed class TeamRepository : BaseCachedRepository<Team>, ITeamRepository
{
    public TeamRepository(
        ICacheService cache,
        IOptions<CacheSettings> cacheSettings,
        IMongoDatabase database)
        : base(cache, cacheSettings, database, "teams")
    {
    }

    public async Task<PagedList<Team>> FindAllTeamsAsync(
        TeamParameters teamParameters,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var cacheKey = $"teams_{teamParameters.PageNumber}_{teamParameters.PageSize}";

        var cached = await Cache.GetAsync<PagedList<Team>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        ct.ThrowIfCancellationRequested();

        var teams = await FindAllAsync(ct);

        var orderedTeams = teams.OrderBy(p => p.Name);

        var pagedTeams = orderedTeams
            .Skip((teamParameters.PageNumber - 1) * teamParameters.PageSize)
            .Take(teamParameters.PageSize)
            .ToList();

        var totalCount = orderedTeams.Count();

        var result = new PagedList<Team>(
            pagedTeams,
            totalCount,
            teamParameters.PageNumber,
            teamParameters.PageSize);

        if (totalCount > 0)
        {
            ct.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                result,
                DefaultCacheExpiration);
        }

        return result;
    }

    public async Task<Team?> GetTeamByTeamIdAsync(string teamId, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        var cacheKey = $"team_{teamId}";

        var cached = await Cache.GetAsync<Team>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        ct.ThrowIfCancellationRequested();

        var filter = Builders<Team>.Filter.Eq(t => t.TeamId, teamId);

        var team = await Collection.Find(filter).FirstOrDefaultAsync(ct);
        if (team != null)
        {
            ct.ThrowIfCancellationRequested();

            await Cache.SetAsync(
                cacheKey,
                team,
                DefaultCacheExpiration);
        }

        return team;
    }

    public override async Task UpdateAsync(Team entity, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.UpdateAsync(entity, ct);

        var cacheKey = $"team_{entity.TeamId}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("teams_");
    }

    public override async Task DeleteAsync(string id, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        await base.DeleteAsync(id, ct);

        var cacheKey = $"team_{id}";
        await Cache.RemoveAsync(cacheKey);

        await Cache.RemoveByPrefixAsync("teams_");
    }
}