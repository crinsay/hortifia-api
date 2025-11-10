using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Infrastructure.Persistence.MigrationManager;

namespace Hortifia.API.Extensions;

public static class ServiceProviderExtensions
{
    public static async Task InitializeInfrastructureAsync(this IServiceProvider serviceProvider)
    {
        await serviceProvider.InitializeDatabaseAsync();
        await serviceProvider.InitializeBlobStorageAsync();
    }

    private static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();

        var migrationManager = scope.ServiceProvider.GetRequiredService<IMigrationManager>();
        await migrationManager.ApplyPendingMigrationsAsync();
    }

    private static async Task InitializeBlobStorageAsync(this IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();

        var blobStorageService = scope.ServiceProvider.GetRequiredService<IBlobStorageService>();
        await blobStorageService.CreateBlobContainerIfNotExistsAsync();
    }
}
