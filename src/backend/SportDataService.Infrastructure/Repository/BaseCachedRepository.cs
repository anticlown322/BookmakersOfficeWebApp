using System.Linq.Expressions;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using SportDataService.Application.Contracts.Services;
using SportDataService.Domain.Models.Settings;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public abstract class BaseCachedRepository<T>
    : ICachedRepository<T>
    where T : class
{
    protected readonly ICacheService Cache;
    protected readonly TimeSpan DefaultCacheExpiration;
    protected readonly IMongoCollection<T> Collection;

    protected BaseCachedRepository(
        ICacheService cache,
        IOptions<CacheSettings> cacheSettings,
        IMongoDatabase database,
        string collectionName)
    {
        Cache = cache;
        DefaultCacheExpiration = TimeSpan.FromMinutes(cacheSettings.Value.DefaultCacheMinutes);
        Collection = database.GetCollection<T>(collectionName);
    }

    public async Task<T?> GetFromCacheOrSourceAsync(
        string cacheKey,
        Func<Task<T?>> sourceFunc,
        TimeSpan? expiration = null)
    {
        var cached = await Cache.GetAsync<T>(cacheKey);
        if (cached != null)
        {
            return cached;
        }

        var result = await sourceFunc();
        if (result != null)
        {
            await Cache.SetAsync(
                cacheKey,
                result,
                expiration ?? DefaultCacheExpiration);
        }

        return result;
    }

    public Task RemoveFromCacheAsync(string cacheKey) => Cache.RemoveAsync(cacheKey);

    public Task RemoveByPrefixAsync(string prefix) => Cache.RemoveByPrefixAsync(prefix);

    public virtual async Task<IEnumerable<T>> FindAllAsync(CancellationToken ct)
        => await Collection.Find(_ => true).ToListAsync(ct);

    public virtual async Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken ct)
        => await Collection.Find(predicate).ToListAsync(ct);

    public virtual async Task<T> GetByIdAsync(string entityId, CancellationToken ct)
    {
        var objectId = ObjectId.Parse(entityId);
        var filter = Builders<T>.Filter.Eq("_id", objectId);
        return await Collection.Find(filter).FirstOrDefaultAsync(ct);
    }

    public virtual async Task CreateAsync(T entity, CancellationToken ct)
    {
        try
        {
            await Collection.InsertOneAsync(entity, cancellationToken: ct);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            throw new InvalidOperationException("Duplicate key violation", ex);
        }
    }

    public virtual async Task UpdateAsync(T entity, CancellationToken ct)
    {
        var id = GetEntityId(entity);
        var objectId = ObjectId.Parse(id);
        var filter = Builders<T>.Filter.Eq("_id", objectId);
        await Collection.ReplaceOneAsync(
            filter,
            entity,
            new ReplaceOptions
            {
                IsUpsert = false,
            },
            ct);
    }

    public virtual async Task DeleteAsync(string entityId, CancellationToken ct)
    {
        var objectId = ObjectId.Parse(entityId);
        var filter = Builders<T>.Filter.Eq("_id", objectId);
        await Collection.DeleteOneAsync(filter, ct);
    }

    private string GetEntityId(T entity)
    {
        var property = typeof(T).GetProperty("Id")
                       ?? throw new InvalidOperationException("Entity must have an Id property");

        return property.GetValue(entity)?.ToString()
               ?? throw new InvalidOperationException("Entity ID cannot be null");
    }
}