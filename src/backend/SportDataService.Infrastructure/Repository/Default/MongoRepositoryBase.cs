using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository.Default;

public abstract class MongoRepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    protected readonly IMongoCollection<T> Collection;

    protected MongoRepositoryBase(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<T>(collectionName);
    }

    public virtual async Task<IEnumerable<T>> FindAllAsync(CancellationToken ct)
        => await Collection.Find(_ => true).ToListAsync(ct);

    public virtual async Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> predicate, CancellationToken ct)
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

    protected virtual string GetEntityId(T entity)
    {
        var property = typeof(T).GetProperty("Id")
                       ?? throw new InvalidOperationException("Entity must have an Id property");

        return property.GetValue(entity)?.ToString()
               ?? throw new InvalidOperationException("Entity ID cannot be null");
    }
}