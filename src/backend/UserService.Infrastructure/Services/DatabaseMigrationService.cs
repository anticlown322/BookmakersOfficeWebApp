using Microsoft.EntityFrameworkCore;
using UserService.Application.Contracts.Services;
using UserService.Infrastructure.Repository;

namespace UserService.Infrastructure.Services;

public class DatabaseMigrationService(RepositoryContext dbContext)
    : IDatabaseMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
        }
    }
}