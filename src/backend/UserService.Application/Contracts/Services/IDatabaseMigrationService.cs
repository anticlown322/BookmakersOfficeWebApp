namespace UserService.Application.Contracts.Services;

public interface IDatabaseMigrationService
{
    Task MigrateAsync(CancellationToken cancellationToken = default);
}