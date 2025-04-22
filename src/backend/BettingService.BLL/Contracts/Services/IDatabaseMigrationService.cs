namespace BettingService.BLL.Contracts.Services;

public interface IDatabaseMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}