using Microsoft.EntityFrameworkCore;

namespace Hortifia.Infrastructure.Persistence.MigrationManager;

internal class MigrationManager(HortifiaDbContext dbContext) : IMigrationManager
{
    public async Task ApplyPendingMigrationsAsync()
    {
        var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await dbContext.Database.MigrateAsync();
        }
    }
}
