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

public sealed class CachedTournamentResultRepository(
    ITournamentResultRepository decorated,
    ICacheService cache,
    IOptions<CacheSettings> cacheSettings)
    : ITournamentResultRepository
{
    public async Task<PagedList<TournamentResult>> FindAllTournamentResultsAsync(
        TournamentResultParameters tournamentResultParameters,
        CancellationToken cancellationToken)
    {
        var cacheKey =
            $"tournament_results_{tournamentResultParameters.PageNumber}_{tournamentResultParameters.PageSize}";

        var cached = await cache.GetAsync<PagedList<TournamentResult>>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.FindAllTournamentResultsAsync(tournamentResultParameters, cancellationToken);

        if (result.Count > 0)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public async Task<TournamentResult?> GetTournamentResultByTournamentResultIdAsync(
        string tournamentResultId,
        CancellationToken ct)
    {
        var cacheKey = $"tournament_result_{tournamentResultId}";

        var cached = await cache.GetAsync<TournamentResult>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await decorated.GetTournamentResultByTournamentResultIdAsync(tournamentResultId, ct);
        if (result != null)
        {
            await cache.SetAsync(
                cacheKey,
                result,
                TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes));
        }

        return result;
    }

    public Task<IEnumerable<TournamentResult>> FindAllAsync(CancellationToken cancellationToken)
        => decorated.FindAllAsync(cancellationToken);

    public Task<IEnumerable<TournamentResult>> FindByConditionAsync(
        Expression<Func<TournamentResult, bool>> expression,
        CancellationToken ct)
        => decorated.FindByConditionAsync(expression, ct);

    public Task<TournamentResult?> GetByIdAsync(string id, CancellationToken ct)
        => decorated.GetByIdAsync(id, ct);

    public Task CreateAsync(TournamentResult entity, CancellationToken ct)
        => decorated.CreateAsync(entity, ct);

    public async Task UpdateAsync(TournamentResult entity, CancellationToken ct)
    {
        await decorated.UpdateAsync(entity, ct);

        var cacheKey = $"tournament_result_{entity.TournamentId}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("tournament_results_");
    }

    public async Task DeleteAsync(string id, CancellationToken ct)
    {
        await decorated.DeleteAsync(id, ct);

        var cacheKey = $"tournament_result_{id}";
        await cache.RemoveAsync(cacheKey);

        await cache.RemoveByPrefixAsync("tournament_results_");
    }
}