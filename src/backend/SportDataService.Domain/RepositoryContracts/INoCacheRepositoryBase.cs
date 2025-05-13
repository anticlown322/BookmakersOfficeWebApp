using System.Linq.Expressions;

namespace SportDataService.Domain.RepositoryContracts;

public interface INoCacheRepositoryBase<T>
    where T : class
{
    Task<IEnumerable<T>> FindByConditionAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);
    Task<T> GetByIdAsync(string entityId, CancellationToken cancellationToken);
}