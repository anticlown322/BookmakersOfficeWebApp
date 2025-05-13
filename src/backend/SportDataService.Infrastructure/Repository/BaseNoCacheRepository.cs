using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Driver;
using SportDataService.Domain.RepositoryContracts;

namespace SportDataService.Infrastructure.Repository;

public abstract class BaseNoCacheRepository<T> : INoCacheRepositoryBase<T>
    where T : class
{
    protected readonly IMongoCollection<T> Collection;

    protected BaseNoCacheRepository(IMongoDatabase database, string collectionName)
    {
        Collection = database.GetCollection<T>(collectionName);
    }

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
}