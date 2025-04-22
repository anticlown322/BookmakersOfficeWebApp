using System.Linq.Expressions;
using BettingService.DAL.Contracts.Repository;
using Microsoft.EntityFrameworkCore;

namespace BettingService.DAL.Repositories;

public abstract class RepositoryBase<T>(
    RepositoryContext repositoryContext) 
    : IRepositoryBase<T>
    where T : class
{
    public async Task<IEnumerable<T>> FindAllAsync(bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges
            ? repositoryContext.Set<T>()
                .AsNoTracking()
            : repositoryContext.Set<T>())
                .ToListAsync(cancellationToken);

    public async Task<IEnumerable<T>> FindByConditionAsync(
        Expression<Func<T, bool>> expression,
        bool trackChanges, CancellationToken cancellationToken) =>
        await (!trackChanges
            ? repositoryContext.Set<T>()
                .Where(expression)
                .AsNoTracking()
            : repositoryContext.Set<T>()
                .Where(expression))
            .ToListAsync(cancellationToken);

    public void Create(T entity) => repositoryContext.Set<T>().Add(entity);
    public void Update(T entity) => repositoryContext.Set<T>().Update(entity);
    public void Delete(T entity) => repositoryContext.Set<T>().Remove(entity);
    public async Task SaveAsync(CancellationToken cancellationToken) =>
        await repositoryContext.SaveChangesAsync(cancellationToken);
}