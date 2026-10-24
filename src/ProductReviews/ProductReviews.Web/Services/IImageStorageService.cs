namespace ProductReviews.Web.Services;

public interface IImageStorageService
{
    Task<string> UploadAsync(Stream content, string extension, string contentType, CancellationToken ct = default);
}
