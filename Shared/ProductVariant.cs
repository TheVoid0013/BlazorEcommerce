using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace BlazorEcommerce.Shared
{
    [Table("ProductVariants")]
    [Index(nameof(ProductId), nameof(ProductTypeId), IsUnique = true, Name = "IX_ProductVariants_Product_ProductType_Unique")]
    [Index(nameof(ProductId), Name = "IX_ProductVariants_ProductId")]
    [Index(nameof(ProductTypeId), Name = "IX_ProductVariants_ProductTypeId")]
    public class ProductVariant
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProductTypeId))]
        public ProductType? ProductType { get; set; }
    }
}