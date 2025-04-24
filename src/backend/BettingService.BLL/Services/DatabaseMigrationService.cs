using BettingService.BLL.Contracts.Services;
using BettingService.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BettingService.BLL.Services;

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