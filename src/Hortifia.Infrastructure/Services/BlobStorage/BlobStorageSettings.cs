namespace Hortifia.Infrastructure.Services.BlobStorage;

internal class BlobStorageSettings
{
    public required string ConnectionString { get; init; }
    public required string ContainerName { get; init; }
}
