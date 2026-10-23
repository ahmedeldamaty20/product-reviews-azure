using System.ComponentModel.DataAnnotations;

namespace ProductReviews.Web.Models;

public class Review
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Author { get; set; } = string.Empty;

    [Range(1, 5)]
    public int Rating { get; set; }

    [Required, StringLength(2000)]
    public string Comment { get; set; } = string.Empty;

    public string? ImageUrl { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}