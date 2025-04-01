using UserService.Domain.RepositoryContracts;

namespace UserService.Infrastructure.Repository;

public sealed class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _repositoryContext;

    public RepositoryManager(RepositoryContext repositoryContext)
    {
        _repositoryContext = repositoryContext;
    }

    public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
}
