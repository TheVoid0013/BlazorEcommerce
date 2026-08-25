using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorEcommerce.Shared
{
    [Table("Products")]
    [Index(nameof(CategoryId), Name = "IX_Products_CategoryId")]
    [Index(nameof(Title), IsUnique = false, Name = "IX_Products_Title")]
    [Index(nameof(Featured), Name = "IX_Products_Featured")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        // Navigation property
        public Category? Category { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public bool Featured { get; set; } = false;

        // For JSON serialization, keep as List
        public List<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    }
}