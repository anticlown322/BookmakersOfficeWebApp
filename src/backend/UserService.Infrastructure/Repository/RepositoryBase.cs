using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.RepositoryContracts;

namespace UserService.Infrastructure.Repository;

public abstract class RepositoryBase<T> : IRepositoryBase<T>
    where T : class
{
    private readonly RepositoryContext _repositoryContext;

    protected RepositoryBase(RepositoryContext repositoryContext)
        => _repositoryContext = repositoryContext;

    public async Task<IEnumerable<T>> FindAllAsync(bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges
            ? _repositoryContext.Set<T>()
                .AsNoTracking()
            : _repositoryContext.Set<T>())
                .ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression,
        bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges
            ? _repositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking()
            : _repositoryContext.Set<T>()
                .Where(expression))
            .ToListAsync(cancellationToken);

    public void Create(T entity) => _repositoryContext.Set<T>().Add(entity);
    public void Update(T entity) => _repositoryContext.Set<T>().Update(entity);
    public void Delete(T entity) => _repositoryContext.Set<T>().Remove(entity);
}