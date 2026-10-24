using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ProductReviews.Web.Services;

public class BlobImageStorageService : IImageStorageService
{
    private readonly BlobContainerClient _container;
    private readonly string? _publicBaseUrl;

    public BlobImageStorageService(BlobServiceClient serviceClient, IConfiguration config)
    {
        var containerName = config["Storage:ContainerName"] ?? "review-images";
        _container = serviceClient.GetBlobContainerClient(containerName);

        // We can optionally configure a public base URL for the blob storage
        // which is useful if we are using a CDN or Azure Front Door in front of the blob storage.
        _publicBaseUrl = config["Storage:PublicBaseUrl"];
    }

    public async Task<string> UploadAsync(Stream content, string extension, string contentType, CancellationToken ct = default)
    {
        // Generate a unique blob name using a GUID and the provided file extension
        var blobName = $"{Guid.NewGuid():N}{extension}";
        var blob = _container.GetBlobClient(blobName);

        var options = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType,
                // Set a long cache duration for the uploaded images (1 year)
                CacheControl = "public, max-age=31536000"
            }
        };

        await blob.UploadAsync(content, options, ct);

        if (string.IsNullOrWhiteSpace(_publicBaseUrl))
            return blob.Uri.ToString();

        return $"{_publicBaseUrl.TrimEnd('/')}/{_container.Name}/{blobName}";
    }
}

