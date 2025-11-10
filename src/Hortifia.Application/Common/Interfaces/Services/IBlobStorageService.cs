namespace Hortifia.Application.Common.Interfaces.Services;

public interface IBlobStorageService
{
    Task UploadBlobAsync(Stream blobContent, string blobName, string contentType);
    Task CreateBlobContainerIfNotExistsAsync();
}
