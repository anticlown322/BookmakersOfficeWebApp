using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserService.Application.Contracts.Services;
using UserService.Infrastructure.Repository;

namespace UserService.Infrastructure.Services;

public class DatabaseMigrationService(
    RepositoryContext dbContext,
    ILogger<DatabaseMigrationService> logger)
    : IDatabaseMigrationService
{
    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Migrating database...");

        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync(cancellationToken);
            logger.LogInformation("Successfully migrated database...");
        }
    }
}