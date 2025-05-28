using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BettingService.BLL.Services;

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