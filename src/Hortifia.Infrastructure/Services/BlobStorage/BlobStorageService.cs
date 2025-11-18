using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Hortifia.Application.Common.Interfaces.Services;
using Hortifia.Application.Common.Types;
using Hortifia.Application.Posts.Queries.GetPosts;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Hortifia.Infrastructure.Services.BlobStorage;

internal class BlobStorageService(IOptions<BlobStorageSettings> options,
    ILogger<BlobStorageService> logger) : IBlobStorageService
{
    private readonly BlobStorageSettings _settings = options.Value;

    public async Task UploadBlobAsync(Stream blobContent, string blobName, string contentType)
    {
        var blobClient = GetBlobClient(blobName);

        // First, for safety reasons we need to check if the blob with such name already exists, and if so, delete it.
        // Unfortunately, the UploadAsync cannot accepts options and overwrite flag at the same time.
        // !!!Important!!!: Such situation shouldn't happen in our application, but extra safety check is always good, especially that it doesn't cost much.
        await blobClient.DeleteIfExistsAsync();

        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            }
        };
        await blobClient.UploadAsync(blobContent, options: blobUploadOptions);
        logger.LogInformation("[{handler}] Succesfully uploaded new blob with name {blobName}.", nameof(BlobStorageService), blobName);
    }

    public async Task DeleteBlobAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
        logger.LogInformation("[{handler}] Succesfully deleted blob with name {blobName}.", nameof(BlobStorageService), blobName);
    }

    public async Task ReplaceBlobAsync(Stream newBlobContent, string newBlobName, string newBlobContentType, string oldBlobName)
    {
        await DeleteBlobAsync(oldBlobName);
        await UploadBlobAsync(newBlobContent, newBlobName, newBlobContentType);
        logger.LogInformation("[{handler}] Succesfully replaced old blob '{oldBlobName}' with new blob '{newBlobName}'.", nameof(BlobStorageService), oldBlobName, newBlobName);
    }

    public async Task<string> GetBlobSasUrlAsync(string blobName)
    {
        var blobClient = GetBlobClient(blobName);

        // !!!Only for safety reason, but in our application this shouldn't happen!!!
        if (!await blobClient.ExistsAsync())
        {
            logger.LogCritical("[{handler}] Failed to generate {blobName} blob sas url. Database might not be in sync with BlobStorage!!!", nameof(BlobStorageService), blobName);
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
