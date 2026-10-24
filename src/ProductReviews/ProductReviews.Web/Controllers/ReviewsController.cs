using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductReviews.Web.Data;
using ProductReviews.Web.Models;
using ProductReviews.Web.Services;

namespace ProductReviews.Web.Controllers;

public class ReviewsController(AppDbContext db, IImageStorageService images) : Controller
{
    private const long MaxImageBytes = 5 * 1024 * 1024; // 5 MB

    private static readonly Dictionary<string, string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"] = "image/jpeg",
        [".jpeg"] = "image/jpeg",
        [".png"] = "image/png",
        [".webp"] = "image/webp"
    };

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(6 * 1024 * 1024)]
    public async Task<IActionResult> Create(
        [Bind("ProductId,Author,Rating,Comment")] Review review,
        IFormFile? image,
        CancellationToken ct)
    {
        var productExists = await db.Products.AnyAsync(p => p.Id == review.ProductId, ct);
        if (!productExists) return NotFound();

        if (!ModelState.IsValid)
            return Fail(review.ProductId, "Please fill in your name, a rating (1-5) and a comment.");

        if (image is { Length: > 0 })
        {
            var extension = Path.GetExtension(image.FileName);

            if (!AllowedImageTypes.TryGetValue(extension, out var contentType))
                return Fail(review.ProductId, "Only JPG, PNG or WEBP images are allowed.");

            if (image.Length > MaxImageBytes)
                return Fail(review.ProductId, "Image is too large (max 5 MB).");

            await using var stream = image.OpenReadStream();
            review.ImageUrl = await images.UploadAsync(stream, extension.ToLowerInvariant(), contentType, ct);
        }

        review.CreatedAt = DateTime.UtcNow;
        db.Reviews.Add(review);
        await db.SaveChangesAsync(ct);

        return RedirectToAction("Details", "Products", new { id = review.ProductId });
    }

    private IActionResult Fail(int productId, string message)
    {
        TempData["Error"] = message;
        return RedirectToAction("Details", "Products", new { id = productId });
    }
}
