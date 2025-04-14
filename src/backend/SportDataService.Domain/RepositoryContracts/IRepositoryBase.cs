using System.Linq.Expressions;

namespace SportDataService.Domain.RepositoryContracts;

public interface IRepositoryBase<T>
    where T : class
{
    Task<IEnumerable<T>> FindAllAsync(CancellationToken cancellationToken);
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(string entityId, CancellationToken cancellationToken);
    Task CreateAsync(T entity, CancellationToken cancellationToken);
    Task UpdateAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(string entityId, CancellationToken cancellationToken);
}