using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductReviews.Web.Data;
using ProductReviews.Web.Models;

namespace ProductReviews.Web.Controllers;

public class ReviewsController : Controller
{
    private readonly AppDbContext _db;

    public ReviewsController(AppDbContext db) => _db = db;

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("ProductId,Author,Rating,Comment")] Review review)
    {
        var productExists = await _db.Products.AnyAsync(p => p.Id == review.ProductId);
        if (!productExists) return NotFound();

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill in your name, a rating (1-5) and a comment.";
            return RedirectToAction("Details", "Products", new { id = review.ProductId });
        }

        review.CreatedAt = DateTime.UtcNow;
        _db.Reviews.Add(review);
        await _db.SaveChangesAsync();

        return RedirectToAction("Details", "Products", new { id = review.ProductId });
    }
}
