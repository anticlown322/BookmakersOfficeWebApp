using System.Linq.Expressions;

namespace SportDataService.Domain.RepositoryContracts;

public interface ICachedRepository<T>
    where T : class
{
    Task<T?> GetFromCacheOrSourceAsync(string cacheKey, Func<Task<T?>> sourceFunc, TimeSpan? expiration = null);
    Task RemoveFromCacheAsync(string cacheKey);
    Task RemoveByPrefixAsync(string prefix);
    Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(string entityId, CancellationToken cancellationToken);
    Task CreateAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(string entityId, CancellationToken cancellationToken);
}