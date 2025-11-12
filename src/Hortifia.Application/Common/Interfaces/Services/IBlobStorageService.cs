namespace Hortifia.Application.Common.Interfaces.Services;

public interface IBlobStorageService
{
    Task UploadBlobAsync(Stream blobContent, string blobName, string contentType);
    Task DeleteBlobAsync(string blobName);
    Task ReplaceBlobAsync(Stream newBlobContent, string newBlobName, string newBlobContentType, string oldBlobName);
    Task<string> GetBlobSasUrlAsync(string blobName);
    Task CreateBlobContainerIfNotExistsAsync();
}
