using Azure.Storage.Blobs;

namespace Hortifia.Infrastructure.Services.BlobStorage.Utils;

internal interface IBlobUrlBuilder
{
    string Build(Uri blobSasUri);
}
