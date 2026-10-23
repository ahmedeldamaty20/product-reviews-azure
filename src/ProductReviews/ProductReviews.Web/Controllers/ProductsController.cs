using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductReviews.Web.Data;
using ProductReviews.Web.Models;

namespace ProductReviews.Web.Controllers;

public class ProductsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var products = await db.Products
            .Include(p => p.Reviews)
            .AsNoTracking()
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return View(products);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null) return NotFound();

        var product = await db.Products
            .Include(p => p.Reviews.OrderByDescending(r => r.CreatedAt))
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product is null) return NotFound();

        return View(product);
    }

    public IActionResult Create() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Description")] Product product)
    {
        if (!ModelState.IsValid) return View(product);

        product.CreatedAt = DateTime.UtcNow;
        db.Products.Add(product);
        await db.SaveChangesAsync();

        return RedirectToAction(nameof(Details), new { id = product.Id });
    }
}
