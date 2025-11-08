namespace Hortifia.Infrastructure.Persistence.MigrationManager;

public interface IMigrationManager
{
    Task ApplyPendingMigrationsAsync();
}
