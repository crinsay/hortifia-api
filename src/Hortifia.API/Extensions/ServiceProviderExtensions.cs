using Hortifia.Infrastructure.Persistence.MigrationManager;

namespace Hortifia.API.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task InitializeInfrastructureAsync(this IServiceProvider serviceProvider)
    {
        await serviceProvider.InitializeDatabaseAsync();
    }

    private static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();

        var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
        await migrationManager.ApplyPendingMigrationsAsync();
    }
}
