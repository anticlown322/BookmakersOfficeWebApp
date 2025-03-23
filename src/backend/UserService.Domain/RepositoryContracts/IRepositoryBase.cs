using System.Linq.Expressions;

namespace UserService.Domain.RepositoryContracts;

public interface IRepositoryBase<T>
{
    Task<IEnumerable<T>> FindAllAsync(bool trackChanges, CancellationToken cancellationToken); 
    Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression, bool trackChanges, CancellationToken cancellationToken);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}