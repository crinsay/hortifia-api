using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Hortifia.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Options;

namespace Hortifia.Infrastructure.Services.BlobStorage;

internal class BlobStorageService(IOptions<BlobStorageSettings> options
    /*ILogger<BlobStorageService> logger*/) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = options.Value;

    public async Task UploadBlobAsync(Stream blobContent, string blobName, string contentType)
    {
        var blobClient = GetBlobClient(blobName);

        // First, we need to check if the blob with such name already exists, and if so, delete it.
        // Unfortunately, the UploadAsync cannot accepts options and overwrite flag at the same time.
        await blobClient.DeleteIfExistsAsync();

        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };
        await blobClient.UploadAsync(blobContent, options: blobUploadOptions);
    }

    public async Task<string> GetBlobSasUrlAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            throw new Exception($"Blob {blobName} not found.");
        }

        var blobSasUri = blobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddHours(1));
        return blobSasUri.ToString();
    }

    public async Task CreateBlobContainerIfNotExistsAsync()
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);
        await blobContainerClient.CreateIfNotExistsAsync();
    }

    private BlobClient GetBlobClient(string blobName)
    {
        var blobServiceClient = new BlobServiceClient(_settings.ConnectionString);

        var blobContainerClient = blobServiceClient.GetBlobContainerClient(_settings.ContainerName);

        var blobClient = blobContainerClient.GetBlobClient(blobName);
        return blobClient;
    }
}
