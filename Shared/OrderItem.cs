using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorEcommerce.Shared
{
    [Table("OrderItems")]
    [Index(nameof(OrderId), Name = "IX_OrderItems_OrderId")]
    [Index(nameof(ProductId), Name = "IX_OrderItems_ProductId")]
    [Index(nameof(OrderId), nameof(ProductId), nameof(ProductTypeId), IsUnique = true, Name = "IX_OrderItems_Order_Product_ProductType_Unique")]
    public class OrderItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        [ForeignKey(nameof(OrderId))]
        public Order? Order { get; set; }

        [ForeignKey(nameof(ProductId))]
        public Product? Product { get; set; }

        [ForeignKey(nameof(ProductTypeId))]
        public ProductType? ProductType { get; set; }
    }
}