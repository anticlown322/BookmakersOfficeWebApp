using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Settings;
using StackExchange.Redis;

namespace SportDataService.Infrastructure.Services.Redis;

public class RedisCacheService(
    IDistributedCache cache,
    IConnectionMultiplexer redis,
    ISerializer serializer,
    IOptions<CacheSettings> settings)
    : ICacheService
{
    public async Task<T?> GetAsync<T>(string key)
    {
        var bytes = await cache.GetAsync(key);
        return bytes == null ? default : serializer.Deserialize<T>(bytes);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(settings.Value.DefaultCacheMinutes),
        };

        var bytes = serializer.Serialize(value);
        await cache.SetAsync(key, bytes, options);
    }

    public async Task RemoveAsync(string key) => await cache.RemoveAsync(key);

    public async Task<bool> ExistsAsync(string key)
    {
        var db = redis.GetDatabase();
        var result = await db.KeyExistsAsync(key);

        return result;
    }

    public async Task RemoveByPrefixAsync(string prefix)
    {
        var db = redis.GetDatabase();
        var endpoints = redis.GetEndPoints();
        var server = redis.GetServer(endpoints[0]);

        await foreach (var key in server.KeysAsync(pattern: $"{prefix}*"))
        {
            await db.KeyDeleteAsync(key);
        }
    }
}