using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Hortifia.Infrastructure.Services.BlobStorage.Utils;

internal sealed class BlobUrlBuilder(IWebHostEnvironment environment, IConfiguration configuration) : IBlobUrlBuilder
{
    private readonly bool _isDevelopment = environment.IsDevelopment();
    private readonly string? _devBlobsPublicHostName = configuration["BlobStorage:DevPublicHostName"];

    public string Build(Uri blobSasUri)
    {
        if (!_isDevelopment || _devBlobsPublicHostName is null)
        {
            return blobSasUri.ToString();
        }

        var uriBuilder = new UriBuilder(blobSasUri)
        {
            Scheme = Uri.UriSchemeHttps,
            Host = _devBlobsPublicHostName,
            Port = -1
        };

        return uriBuilder.Uri.ToString();
    }
}
